using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoneyPilot.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MoneyPilot.Infrastructure.Services
{
    public class AutoLoginTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly TestUserService _testUserService;

        public AutoLoginTokenService(
            IConfiguration configuration,
            UserManager<AppUser> userManager,
            TestUserService testUserService)
        {
            _configuration = configuration;
            _userManager = userManager;
            _testUserService = testUserService;
        }

        public async Task<string?> GenerateAutoLoginTokenAsync()
        {
            // Check if auto-login is enabled
            var autoLoginValue = _configuration["TestUser:AutoLogin"];
            if (string.IsNullOrEmpty(autoLoginValue) || !bool.TryParse(autoLoginValue, out bool autoLogin) || !autoLogin)
                return null;

            // Ensure test user exists
            var user = await _testUserService.EnsureTestUserCreatedAsync();
            if (user == null)
                return null;

            // Generate token
            var token = await GenerateJwtTokenAsync(user);
            return token;
        }

        private async Task<string> GenerateJwtTokenAsync(AppUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            // Add user roles to claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}