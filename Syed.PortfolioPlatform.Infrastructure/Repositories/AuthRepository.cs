using Microsoft.EntityFrameworkCore;
using Syed.PortfolioPlatform.Application.Common.Interface;
using Syed.PortfolioPlatform.Domain.Entities;
using Syed.PortfolioPlatform.Infrastructure.Persistence;

public class AuthRepository : IAuthRepository
{
    private readonly ApplicationDbContext _context;

    public AuthRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAdminAsync(AdminUser admin)
    {
        _context.AdminUsers.Add(admin);
        await _context.SaveChangesAsync();
    }

    public async Task<AdminUser?> GetAdminByUsernameAsync(string username)
    {
        return await _context.AdminUsers
            .Include(a => a.RoleNavigation)
            .FirstOrDefaultAsync(a => a.Username == username);
    }

    public async Task AddRefreshTokenAsync(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string hashedToken)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.Admin)
            .ThenInclude(a => a.RoleNavigation)
            .FirstOrDefaultAsync(rt => rt.Token == hashedToken);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}