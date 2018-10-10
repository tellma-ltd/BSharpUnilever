using BSharpUnilever.Controllers.Util;
using BSharpUnilever.Controllers.ViewModels.Auth;
using BSharpUnilever.Data.Entities;
using BSharpUnilever.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace BSharpUnilever.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // All issued tokens expire after 7 days, even though we intend
        // to keep users permanently signed in, it is still a good idea to
        // set an expiry date and use a refresh endpoint. This
        // ensures that there will be no lingering valid tokens on
        // e.g. stolen phones that remain valid after decades!
        private const double TOKEN_EXPIRY_DAYS = 7.0;

        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;
        private readonly IEmailSender _emailSender;

        public AuthController(UserManager<User> userManager, IConfiguration config, ILogger<AuthController> logger, IEmailSender emailSender)
        {
            _userManager = userManager;
            _config = config;
            _logger = logger;
            _emailSender = emailSender;
        }

        [HttpPost("create-token")]
        public async Task<ActionResult<AuthenticationTokenResponseVM>> CreateToken([FromBody] CreateTokenVM model)
        {
            // This is the endpoint where users sign in

            if (!ModelState.IsValid)
            {
                return BadRequest("There is something wrong with the request payload"); // TODO: Return friendlier validation errors
            }

            try
            {
                // To get a valid token the user (1) must exist
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    return BadRequest("Invalid email and password combination"); // The knowledge of which one is on-a-need-to-know-basis
                }

                //(2) have a confirmed email
                bool confirmedEmail = await _userManager.IsEmailConfirmedAsync(user);
                if (!confirmedEmail)
                {
                    return BadRequest("This email is not confirmed yet, please go to your email inbox and find the email confirmation link that we sent you");
                }

                // (3) and supply a valid password
                bool validPassword = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!validPassword)
                {
                    return BadRequest("Invalid email and password combination"); // The knowledge of which one is on-a-need-to-know-basis
                }

                // If we reach here, the user is authentic, return an authentication back
                var tokenResponse = CreateJwtToken(user.UserName);
                return Created("", tokenResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [Authorize] // IMPORTANT!
        [HttpGet("refresh-token")]
        public ActionResult<AuthenticationTokenResponseVM> RefreshToken()
        {
            // The client web app uses this endpoint to refresh the token every 1h in order to 
            // keep the user session alive permanently as long as the client remains open
            try
            {
                string userEmail = User.Username();
                var tokenResponse = CreateJwtToken(userEmail);
                return Created("", tokenResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("confirm-email")]
        public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailVM model)
        {
            // This is called when the user presses the confirm email link that is sent to his/her inbox 
            if (!ModelState.IsValid)
            {
                return BadRequest("There is something wrong with the request payload"); // TODO: Return friendlier validation errors
            }

            try
            {
                // Get the user
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    // This is only possible if the user changes the email confirmation link before clicking
                    return BadRequest("The supplied user id could not be found");
                }

                // Rely on the injected user manager to validate the token and confirm the user's email
                IdentityResult result = await _userManager.ConfirmEmailAsync(user, model.EmailConfirmationToken);
                if (!result.Succeeded)
                {
                    return BadRequest(result.ErrorMessage("Could not confirm email"));
                }

                // All good
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordVM model)
        {
            // This is called when the user clicks "forgot my password", and then submits an email address
            if (!ModelState.IsValid)
            {
                return BadRequest("There is something wrong with the request payload"); // TODO: Return friendlier validation errors
            }

            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    return BadRequest("Could not find the specified email");
                }

                // Rely on the injected user manager to generate a token
                string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                // In a bigger app, the SPA and the API should be independent and the API can't't know where
                // the SPA is and the setup below would have to change, but for our purposes this is fine for
                // now and a lot easier as the SPA is available and already comes with all the styling
                string uri = Url.Content("~/forgot-password");
                uri = QueryHelpers.AddQueryString(uri, "userId", user.Id);
                uri = QueryHelpers.AddQueryString(uri, "passwordResetToken", passwordResetToken);
                var htmlUri = HtmlEncoder.Default.Encode(uri);

                // Prepare the email template (in a larger app you would probably store such templates in files)
                string htmlEmail = Util.Util.TranscludeInBSharpEmailTemplate(
                                  $@"<p>
                                        <br /> 
                                        To reset your password please click below
                                        <br />
                                        <br />
                                        <br />
                                        <a href=""{htmlUri}"" style=""background-color:#17a2b8;padding:20px;text-decoration:none;color:#fff"">
                                            Reset My Password
                                        </a>
                                    </p>");

                // Send the email using injected sender
                await _emailSender.SendEmail(
                    destinationEmailAddress: model.Email,
                    subject: "Password Reset Link",
                    htmlEmail: htmlEmail);

                // All good
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordVM model)
        {
            // This is called when the user clicks "forgot my password", and then submits an email address
            if (!ModelState.IsValid)
            {
                return BadRequest("There is something wrong with the request payload"); // TODO: Return friendlier validation errors
            }

            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    // This is only possible if the user somehow changes the password reset confirmation link before clicking
                    return BadRequest("The supplied user id could not be found");
                }

                // Reset the password by utilizing the injected user manager
                IdentityResult result = await _userManager.ResetPasswordAsync(user, model.PasswordResetToken, model.NewPassword);
                if (result.Succeeded)
                {
                    return BadRequest(result.ErrorMessage("Could not reset password"));
                }
                else
                {
                    // If the password reset succeeds, automatically log the user in
                    var tokenResponse = CreateJwtToken(user.Email);
                    return Created("", tokenResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// A helper method to create a JWT authentication token response object
        /// </summary>
        /// <param name="email">The email of the authenticated user</param>
        /// <returns></returns>
        private AuthenticationTokenResponseVM CreateJwtToken(string email)
        {
            // A uniqe Id for this particular token
            var jti = Guid.NewGuid().ToString();

            // These claims are the minimum requirement for a valid JWT token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            // Sign the token with our super secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Tokens:Issuer"],
                audience: _config["Tokens:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(TOKEN_EXPIRY_DAYS),
                signingCredentials: signingCredentials);
            

            // Return the token wrapped inside a friendly data structure
            var tokenResponse = new AuthenticationTokenResponseVM
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                Email = email,
                Jti = jti
            };

            return tokenResponse;
        }
    }
}
