using Lib.Infrastructure.App;
using Lib.Modules.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Users.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id)
    {
        return db.Set<User>()
            .Include(u => u.Avatar)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return db.Set<User>()
            .Include(u => u.Avatar)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        return db.Set<User>()
            .Include(u => u.Avatar)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public Task<bool> ExistsByEmailAsync(string email)
    {
        return db.Set<User>().AnyAsync(u => u.Email == email);
    }

    public Task<bool> ExistsByUsernameAsync(string username)
    {
        return db.Set<User>().AnyAsync(u => u.Username == username);
    }

    public async Task AddAsync(User user)
    {
        await db.Set<User>().AddAsync(user);
    }

    public void Delete(User user)
    {
        db.Set<User>().Remove(user);
    }

    public Task SaveChangesAsync()
    {
        return db.SaveChangesAsync();
    }
}