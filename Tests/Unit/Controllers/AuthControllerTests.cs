using API.Controllers;
using Application.Dtos.Auth;
using Application.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        public void Login_ValidCredentials_ReturnsOkWithToken()
        {
            var dto = new LoginDto { Username = "Prueba", Password = "PruebaRomar" };
            var tokenDto = new ResponseTokenDto { Token = "abc", Expiration = DateTime.UtcNow.AddHours(1) };

            var mock = new Mock<IAuthService>();
            mock.Setup(s => s.Login(dto)).Returns(tokenDto);

            var ctrl = new AuthController(mock.Object);
            var result = ctrl.Login(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(tokenDto, ok.Value);
        }

        [Fact]
        public void Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var dto = new LoginDto { Username = "X", Password = "Y" };

            var mock = new Mock<IAuthService>();
            mock.Setup(s => s.Login(dto)).Returns((ResponseTokenDto?)null);

            var ctrl = new AuthController(mock.Object);
            var result = ctrl.Login(dto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
