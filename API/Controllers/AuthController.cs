using Application.Dtos.Auth;
using Application.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var token = _authService.Login(dto);
            if (token == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(token);
        }
    }
}
