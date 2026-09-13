using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LearnLink.Services
{
    public class JwtService
    {
        private readonly string secret;

        public JwtService()
        {
            secret = ConfigurationManager.AppSettings["JwtSecret"];
        }

        public string GenerateToken(
            int userId,
            string userName,
            string email,
            string role)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secret)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    userId.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    userName
                ),

                new Claim(
                    ClaimTypes.Email,
                    email
                ),

                new Claim(
                    ClaimTypes.Role,
                    role
                )
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}