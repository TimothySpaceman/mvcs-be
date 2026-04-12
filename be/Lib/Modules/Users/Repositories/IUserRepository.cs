using Lib.Modules.Users.Entities;

namespace Lib.Modules.Users.Repositories;

public interface IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id);
    public Task<User?> GetByEmailAsync(string email);
    public Task<User?> GetByUsernameAsync(string username);
    public Task<bool> ExistsByEmailAsync(string email);
    public Task<bool> ExistsByUsernameAsync(string username);
    public Task AddAsync(User user);
    public void Delete(User user);
    public Task SaveChangesAsync();
}