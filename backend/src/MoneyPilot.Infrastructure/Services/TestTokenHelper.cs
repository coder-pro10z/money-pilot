using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoneyPilot.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MoneyPilot.Infrastructure.Services
{
    public class TestTokenHelper
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly TestUserService _testUserService;

        public TestTokenHelper(
            IConfiguration configuration,
            UserManager<AppUser> userManager,
            TestUserService testUserService)
        {
            _configuration = configuration;
            _userManager = userManager;
            _testUserService = testUserService;
        }

        public async Task<string?> GenerateTokenForTestUserAsync(string email)
        {
            // Ensure the test user exists
            var user = await _testUserService.EnsureTestUserCreatedAsync();
            if (user == null)
                throw new Exception("Test user could not be created or found.");

            // Generate JWT token
            var token = await GenerateJwtTokenAsync(user);
            return token;
        }

        private async Task<string> GenerateJwtTokenAsync(AppUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyString = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is missing");
            var issuer = jwtSettings["Issuer"] ?? "MoneyPilotAPI";
            var audience = jwtSettings["Audience"] ?? "MoneyPilotUsers";
            var expiresMinutes = jwtSettings["ExpiresInMinutes"] ?? "60";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("auto_login", "true")
            };

            // Add roles
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(expiresMinutes)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<dynamic?> GetTestUserInfoAsync()
        {
            var user = await _testUserService.EnsureTestUserCreatedAsync();
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new
            {
                user.Id,
                user.Email,
                user.UserName,
                Roles = roles
            };
        }
    }
}