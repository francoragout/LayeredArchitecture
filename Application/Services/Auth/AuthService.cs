using Application.Dtos.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public ResponseTokenDto? Login(LoginDto dto)
        {
            var configUser = _config["AuthSettings:Username"];
            var configPass = _config["AuthSettings:Password"];

            if (string.IsNullOrEmpty(configUser) || string.IsNullOrEmpty(configPass))
                return null;

            if (!string.Equals(dto.Username, configUser, StringComparison.Ordinal) ||
                !string.Equals(dto.Password, configPass, StringComparison.Ordinal))
            {
                return null;
            }

            var jwtKey = _config["Jwt:Key"] ?? string.Empty;
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            // Ensure key is at least 256 bits. If too short, derive a 256-bit key by hashing.
            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
            if (keyBytes.Length < 32)
            {
                keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(jwtKey));
            }

            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, dto.Username)
            };

            var expires = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new ResponseTokenDto
            {
                Token = tokenString,
                Expiration = expires
            };
        }
    }
}
