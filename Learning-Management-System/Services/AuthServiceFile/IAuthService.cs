using Learning_Management_System.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace Learning_Management_System.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto dto);
    }
}
