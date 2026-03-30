using Lib.Infrastructure.App;
using Lib.Modules.Auth.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Auth.Repositories;

public class UserCredentialsRepository(AppDbContext db) : IUserCredentialsRepository
{
    public Task<UserCredentials?> GetByIdAsync(Guid id)
    {
        return db.Set<UserCredentials>()
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task<UserCredentials?> GetByUserIdAsync(Guid userId)
    {
        return db.Set<UserCredentials>()
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public Task<bool> ExistsByUserIdAsync(Guid userId)
    {
        return db.Set<UserCredentials>().AnyAsync(c => c.UserId == userId);
    }

    public async Task AddAsync(UserCredentials credentials)
    {
        await db.Set<UserCredentials>().AddAsync(credentials);
    }

    public void Delete(UserCredentials credentials)
    {
        db.Set<UserCredentials>().Remove(credentials);
    }

    public Task SaveChangesAsync()
    {
        return db.SaveChangesAsync();
    }
}