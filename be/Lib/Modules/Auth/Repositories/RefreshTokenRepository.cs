using Lib.Infrastructure.App;
using Lib.Modules.Auth.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Auth.Repositories;

public class RefreshTokenRepository(AppDbContext db) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByIdAsync(Guid id)
    {
        return db.Set<RefreshToken>()
            .Include(r => r.Session)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public Task<RefreshToken?> GetBySessionIdAsync(Guid sessionId)
    {
        return db.Set<RefreshToken>()
            .Include(r => r.Session)
            .FirstOrDefaultAsync(r => r.SessionId == sessionId);
    }

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
    {
        return db.Set<RefreshToken>()
            .Include(r => r.Session)
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash);
    }

    public Task<bool> ExistsBySessionIdAsync(Guid sessionId)
    {
        return db.Set<RefreshToken>().AnyAsync(r => r.SessionId == sessionId);
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await db.Set<RefreshToken>().AddAsync(refreshToken);
    }

    public void Delete(RefreshToken refreshToken)
    {
        db.Set<RefreshToken>().Remove(refreshToken);
    }

    public Task SaveChangesAsync()
    {
        return db.SaveChangesAsync();
    }
}