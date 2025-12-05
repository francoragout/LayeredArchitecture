using Application.Dtos.Auth;
using Application.Services.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Unit.Services
{
    public class AuthServiceTests
    {
        private IConfiguration BuildConfiguration(string key = "uG7f9sK3v2Zq1Xy8P0rT4mN6bVwE5aR1hQ==")
        {
            var inMemory = new Dictionary<string, string?>
            {
                ["AuthSettings:Username"] = "Prueba",
                ["AuthSettings:Password"] = "PruebaRomar",
                ["Jwt:Key"] = key,
                ["Jwt:Issuer"] = "https://localhost:7150",
                ["Jwt:Audience"] = "api://layered-architecture"
            };
            return new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();
        }

        [Fact]
        public void Login_WithValidCredentials_ReturnsValidToken()
        {
            var config = BuildConfiguration();
            var svc = new AuthService(config);

            var dto = new LoginDto { Username = "Prueba", Password = "PruebaRomar" };
            var result = svc.Login(dto);

            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result!.Token));
            Assert.True(result.Expiration > DateTime.UtcNow);

            // Validate token signature and claims
            var handler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = config["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateLifetime = true
            };

            var principal = handler.ValidateToken(result.Token, validationParameters, out var validatedToken);
            Assert.Equal("Prueba", principal.Identity?.Name);
        }

        [Fact]
        public void Login_WithInvalidCredentials_ReturnsNull()
        {
            var config = BuildConfiguration();
            var svc = new AuthService(config);

            var dto = new LoginDto { Username = "Prueba", Password = "wrong" };
            var result = svc.Login(dto);

            Assert.Null(result);
        }
    }
}
