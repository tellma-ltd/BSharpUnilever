using BSharpUnilever.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace BSharpUnilever.Data
{
    /// <summary>
    /// This class is responsible for any seeding when the server starts
    /// </summary>
    public class BSharpContextSeeder
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;

        public BSharpContextSeeder(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IHostingEnvironment env, IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
            _config = config;
        }

        /// <summary>
        /// Seeds the context with the initial data if the context is empty
        /// </summary>
        /// <returns></returns>
        public async Task CreateAdminUserAndRolesAsync()
        {
            // Create the first administrator user who will bootstrap the others
            string adminEmail = "support@banan-it.com";
            string adminFullName = "Banan Support";
            User adminUser = await CreateAdminUser(adminEmail, adminFullName);

            // If it's development environment, optionally add the developer's personal email from configuration, to help debug email functionality
            if (_env.IsDevelopment())
            {
                string devEmail = _config["DeveloperEmail"];
                string devFullName = _config["DeveloperFullName"];

                if (devEmail != null && devFullName != null)
                {
                    await CreateAdminUser(devEmail, devFullName);
                }
            }
        }

        private async Task<User> CreateAdminUser(string email, string fullName)
        {
            User user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FullName = fullName,
                    Email = email,
                    UserName = email,
                    Role = "Administrator",
                    EmailConfirmed = true
                };

                IdentityResult result = await _userManager.CreateAsync(user, "Banan@123"); // Must be reset immediately after first deployment
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException("Could not create user in seeder");
                }
            }

            return user;
        }

        // Helper method for creating roles
        private async Task<IdentityRole> CreateRole(string roleName)
        {
            IdentityRole role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                role = new IdentityRole(roleName);

                IdentityResult result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Could not create role {roleName} in seeder");
                }
            }

            return role;
        }
    }
}
