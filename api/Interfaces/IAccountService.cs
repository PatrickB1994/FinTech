using api.Dtos.Account;

namespace api.Interfaces
{
    public interface IAccountService
    {
        Task<NewUserDto> Login(LoginDto loginDto);
        Task<NewUserDto> Register(RegisterDto registerDto);
    }
}