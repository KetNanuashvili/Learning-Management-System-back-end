using Learning_Management_System.Models;

namespace Learning_Management_System.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        RefreshToken GenerateRefreshToken();
        Task SaveRefreshTokenAsync(RefreshToken token);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task ReplaceRefreshTokenAsync(RefreshToken oldToken, RefreshToken newToken);
    }
}
