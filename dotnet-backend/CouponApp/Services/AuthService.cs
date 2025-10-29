using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using CouponApp.Models;
using CouponApp.DTOs;

namespace CouponApp.Services
{
    public class AuthService
    {
        private readonly string _secretKey;
        private readonly int _tokenExpiryMinutes;
        private readonly LdapService _ldapService;

        public AuthService(IConfiguration configuration, LdapService ldapService)
        {
            _secretKey = configuration["Jwt:Key"] ?? "default_secret_key_that_should_be_changed_in_production";
            _tokenExpiryMinutes = int.Parse(configuration["Jwt:ExpiryMinutes"] ?? "30");
            _ldapService = ldapService;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("UserId", user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(_tokenExpiryMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<bool> AuthenticateWithLdap(string username, string password)
        {
            return await _ldapService.AuthenticateUserAsync(username, password);
        }
    }
}