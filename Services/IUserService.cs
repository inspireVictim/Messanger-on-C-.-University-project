using messanger.DTOs;

namespace messanger.Services
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
    }
}
