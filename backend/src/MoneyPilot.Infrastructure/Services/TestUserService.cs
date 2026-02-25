using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoneyPilot.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyPilot.Infrastructure.Services
{
    public class TestUserService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<TestUserService> _logger;

        public TestUserService(
            IConfiguration configuration,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<TestUserService> logger)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<AppUser?> EnsureTestUserCreatedAsync()
        {
            // FIX: Use GetSection with indexer and TryParse
            var testUserSection = _configuration.GetSection("TestUser");
            var enabledValue = testUserSection["Enabled"];

            if (string.IsNullOrEmpty(enabledValue) || !bool.TryParse(enabledValue, out bool enabled) || !enabled)
                return null;

            var email = testUserSection["Email"] ?? "test@email.com";
            var password = testUserSection["Password"] ?? "Test@123";

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    _logger.LogError("Failed to create test user: {Errors}",
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    return null;
                }

                _logger.LogInformation("✅ Created test user: {Email}", email);

                // Get roles from configuration - FIXED approach
                var roles = new[] { "User" };
                var rolesValue = testUserSection["Roles"];

                if (!string.IsNullOrEmpty(rolesValue))
                {
                    // Simple comma-separated list: "User,Admin" or single "User"
                    roles = rolesValue.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(r => r.Trim())
                                      .ToArray();
                }

                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                        await _roleManager.CreateAsync(new IdentityRole(role));

                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            return user;
        }
    }
}