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

    public async Task<List<Session>> GetPageByUserIdAsync(Guid userId, Guid? beforeId, int limit)
    {
        var query = db.Set<Session>()
            .Include(s => s.RefreshToken)
            .Where(s => s.UserId == userId);

        if (beforeId is not null)
        {
            var beforeDate = await db.Set<Session>()
                .Where(s => s.Id == beforeId)
                .Select(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            query = query.Where(s => s.CreatedAt < beforeDate);
        }

        return await query
            .OrderByDescending(s => s.CreatedAt)
            .Take(limit)
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