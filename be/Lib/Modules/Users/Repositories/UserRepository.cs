using Lib.Infrastructure.App;
using Lib.Modules.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Users.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<List<User>> GetAllAsync(UserFilter filter)
    {
        var query = db.Set<User>().Include(u => u.Avatar).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim().ToLower();
            query = query.Where(u =>
                u.Username.ToLower().Contains(term) ||
                u.DisplayName.ToLower().Contains(term) ||
                u.Email.ToLower().Contains(term)
            );
        }

        return query
            .OrderBy(u => u.CreatedAt)
            .Skip((filter.Page - 1) * filter.ItemsPerPage)
            .Take(filter.ItemsPerPage)
            .ToListAsync();
    }

    public Task<int> CountAsync(UserFilter filter)
    {
        var query = db.Set<User>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim().ToLower();
            query = query.Where(u =>
                u.Username.ToLower().Contains(term) ||
                u.DisplayName.ToLower().Contains(term) ||
                u.Email.ToLower().Contains(term)
            );
        }

        return query.CountAsync();
    }

    public Task<List<User>> GetAllByIdsAsync(IEnumerable<Guid> ids)
    {
        return db.Set<User>()
            .Include(u => u.Avatar)
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();
    }
    
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