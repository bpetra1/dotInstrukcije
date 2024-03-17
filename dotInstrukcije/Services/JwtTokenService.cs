using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace dotInstrukcije.Services
{
    public class JwtTokenService
    {
        private readonly string _secretKey;
        private readonly int _expiryHours;

        public JwtTokenService(string secretKey, int expiryHours)
        {
            _secretKey = secretKey;
            _expiryHours = expiryHours;
        }

        public string GenerateToken(string userEmail)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, userEmail),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(_expiryHours),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
