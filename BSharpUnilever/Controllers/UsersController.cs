using AutoMapper;
using BSharpUnilever.Controllers.Util;
using BSharpUnilever.Controllers.ViewModels;
using BSharpUnilever.Data;
using BSharpUnilever.Data.Entities;
using BSharpUnilever.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace BSharpUnilever.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private const int DEFAULT_PAGE_SIZE = 50;
        private const int MAX_PAGE_SIZE = 5000;

        private readonly BSharpContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<User> _userManager;

        public UsersController(BSharpContext context, IMapper mapper, ILogger<UsersController> logger,
            IEmailSender emailSender, UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserVM>> GetAll(int top = DEFAULT_PAGE_SIZE,
                                                        int skip = 0,
                                                        string orderby = nameof(UserVM.FullName),
                                                        bool desc = false,
                                                        string search = null)
        {
            try
            {
                // First get a readonly query
                IQueryable<User> query = _context.Users.AsNoTracking();

                // Apply the searching
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(e => e.FullName.Contains(search) || e.Email.Contains(search));
                }

                // Before ordering or paging, retrieve the total count
                int totalCount = query.Count();

                // Apply the ordering (If orderby does not exist the system just ignores it)
                if (!string.IsNullOrWhiteSpace(orderby) && typeof(UserVM).HasProperty(orderby))
                {
                    string key = orderby; // TODO get the mapped property from the entity
                    query = query.OrderBy(key, isDescending: desc);
                }

                // Apply the paging (Protect against DOS attacks by enforcing a maximum page size)
                top = Math.Min(top, MAX_PAGE_SIZE);
                query = query.Skip(skip).Take(top);

                // Load the data, transform it and wrap it in some metadata
                var memoryList = query.ToList();
                var resultData = _mapper.Map<List<User>, List<UserVM>>(memoryList);
                var result = new ListResultVM<UserVM>
                {
                    Skip = skip,
                    Top = resultData.Count,
                    OrderBy = orderby,
                    Desc = desc,
                    TotalCount = totalCount,
                    Data = resultData
                };

                // Finally return the result
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserVM>> Get(string id)
        {
            try
            {
                // Retrieve the user and if missing return a 404
                User user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"Could not find a user with an Id of '{id}'");
                }

                // All is good
                return Ok(_mapper.Map<User, UserVM>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<UserVM>> Post(UserVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("There is something wrong with the request payload"); // TODO: Return friendlier validation errors
            }

            try
            {
                if (string.IsNullOrWhiteSpace(model.Id)) // Insert logic
                {
                    User user = new User
                    {
                        FullName = model.FullName,
                        UserName = model.Email,
                        Email = model.Email
                    };

                    // Create the user object in the identity store
                    IdentityResult result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        return BadRequest(result.ErrorMessage("Could not create user"));
                    }

                    // Rely on the injected user manager to generate the email-confirmation and password-reset tokens
                    string emailConfirmationToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // In a bigger app, the API should not know where the SPA lives
                    // But this is fine and convenient for now
                    string uri = Url.Content("~/confirm-email");
                    uri = QueryHelpers.AddQueryString(uri, "userId", user.Id);
                    uri = QueryHelpers.AddQueryString(uri, "emailConfirmationToken", emailConfirmationToken);
                    uri = QueryHelpers.AddQueryString(uri, "passwordResetToken", passwordResetToken);
                    string currentUser = User.UserName();

                    // Prepare the email content
                    string htmlEmail = Util.Util.BSharpEmailTemplate(
                        message: $"{currentUser} is inviting you to join! Let's start by confirming your email:",
                        hrefToAction: uri, 
                        hrefLabel: "Confirm My Email");

                    // Send the email using injected sender
                    await _emailSender.SendEmail(
                        destinationEmailAddress: model.Email,
                        subject: "Invitation to join BSharp ERP",
                        htmlEmail: htmlEmail);


                    // Map the newly created User object, and return that
                    var resultModel = _mapper.Map<User, UserVM>(user);
                    var resourceUrl = $"{Url.Action(nameof(Get), nameof(UsersController))}/{resultModel.Id}"; // TODO fix this bug

                    return Created(resourceUrl, resultModel);
                }
                else  // Update logic
                {                   
                    User user = await _userManager.FindByIdAsync(model.Id);
                    if (user == null)
                    {
                        return NotFound($"Could not find a user with an Id of '{model.Id}'");
                    }

                    // Here we ensure that there is at least one administrator in the system at all times
                    if(model.Email == User.UserName() && model.Role != Roles.Administrator)
                    {
                        return BadRequest("You cannot remove the administrator role from yourself");
                    }

                    // Update the updatable properties and ignore the rest
                    user.FullName = model.FullName;
                    user.Role = model.Role;


                    // Update in identity store using the userManager
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        return BadRequest(result.ErrorMessage("Could not update user"));
                    }

                    // Finally return the result
                    var resultModel = _mapper.Map<User, UserVM>(user);
                    return Ok(resultModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Missing argument 'id'");

            try
            {
                // Fetch the user to delete
                var userToDelete = await _userManager.FindByIdAsync(id);
                if(userToDelete == null)
                {
                    return NotFound($"Could not find a user with an Id of '{id}'");
                }

                // Rely on the injected _userManager to delete the user
                var result = await _userManager.DeleteAsync(userToDelete);
                if(!result.Succeeded)
                {
                    return BadRequest(result.ErrorMessage("Could not delete user"));
                }

                // Return the result
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex);
            }
        }
    }
}
