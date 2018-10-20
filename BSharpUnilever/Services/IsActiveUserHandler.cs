using BSharpUnilever.Controllers.Util;
using BSharpUnilever.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSharpUnilever.Services
{
    /// <summary>
    /// The two classes below together with a policy "Active" registered in the Startup.ConfigureServices
    /// give us a friendly and readable way to decorate endpoints that only active users can access
    /// </summary>
    public class IsActiveUserHandler : AuthorizationHandler<IsActiveUserRequirement>
    {
        private readonly UserManager<User> _userManager;

        public IsActiveUserHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsActiveUserRequirement requirement)
        {
            // Retrieve the user object using the injected user manager, and check its role
            string username = context.User.UserName();
            var user = await _userManager.FindByNameAsync(username);

            if (user != null && user.Role != Roles.Inactive)
                context.Succeed(requirement);
        }
    }

    /// <summary>
    /// Authorization requirement mandating that the current user be active
    /// </summary>
    public class IsActiveUserRequirement : IAuthorizationRequirement
    {
    }
}
