using Application.Dtos.Auth;

namespace Application.Services.Auth
{
    public interface IAuthService
    {
        ResponseTokenDto? Login(LoginDto dto );
    }
}