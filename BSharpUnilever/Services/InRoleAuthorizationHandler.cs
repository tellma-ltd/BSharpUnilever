using BSharpUnilever.Controllers.Util;
using BSharpUnilever.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BSharpUnilever.Services
{
    /// <summary>
    /// The two classes below together with a policy "AdminOnly" registered in the Startup.ConfigureServices
    /// give us a friendly and readable way to decorate endpoints that only the Administrator can access
    /// </summary>
    public class IsAdministratorHandler : AuthorizationHandler<IsAdministratorRequirement>
    {
        private readonly UserManager<User> _userManager;

        public IsAdministratorHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdministratorRequirement requirement)
        {
            // Retrieve the user object using the injected user manager, and check its role
            string username = context.User.Username();
            var user = await _userManager.FindByNameAsync(username);

            if (user != null && user.Role == "Administrator")
                context.Succeed(requirement);
        }
    }

    /// <summary>
    /// Authorization requirement mandating that the current user hold the Administrator role
    /// </summary>
    public class IsAdministratorRequirement : IAuthorizationRequirement
    {
    }
}
