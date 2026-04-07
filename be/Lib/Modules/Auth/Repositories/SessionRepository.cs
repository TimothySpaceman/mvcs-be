using Lib.Infrastructure.App;
using Lib.Modules.Auth.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Auth.Repositories;

public class SessionRepository(AppDbContext db) : ISessionRepository
{
    public Task<Session?> GetByIdAsync(Guid id)
    {
        return db.Set<Session>()
            .Include(s => s.User)
            .Include(s => s.RefreshToken)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public Task<List<Session>> GetByUserIdAsync(Guid userId)
    {
        return db.Set<Session>()
            .Include(s => s.RefreshToken)
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }

    public Task<Session?> GetByTokenHashAsync(string tokenHash)
    {
        return db.Set<Session>()
            .Include(s => s.User)
            .Include(s => s.RefreshToken)
            .FirstOrDefaultAsync(s => s.RefreshToken.TokenHash == tokenHash);
    }

    public async Task AddAsync(Session session)
    {
        await db.Set<Session>().AddAsync(session);
    }

    public void Delete(Session session)
    {
        db.Set<Session>().Remove(session);
    }

    public Task SaveChangesAsync()
    {
        return db.SaveChangesAsync();
    }
}