using YC5.DTOs;

namespace YC5.Interfaces
{
    public interface IAuthService
    {
        Task<string> Register(RegisterDto dto);
        Task<string?> Login(LoginDto dto); // Trả về Token hoặc null nếu thất bại
    }
}