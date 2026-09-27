using Syed.PortfolioPlatform.Domain.Entities;

namespace Syed.PortfolioPlatform.Application.Common.Interface
{
    public interface IAuthRepository
    {
        Task AddAdminAsync(AdminUser admin);
        Task<AdminUser?> GetAdminByUsernameAsync(string username);
        Task AddRefreshTokenAsync(RefreshToken token);
        Task<RefreshToken?> GetRefreshTokenAsync(string hashedToken);
        Task SaveChangesAsync();
    }
}
