using Lib.Modules.Users.Entities;

namespace Lib.Modules.Users.Repositories;

public interface IUserRepository
{
    public Task<List<User>> GetAllAsync(UserFilter filter);
    public Task<int> CountAsync(UserFilter filter);
    public Task<List<User>> GetAllByIdsAsync(IEnumerable<Guid> ids);
    public Task<User?> GetByIdAsync(Guid id);
    public Task<User?> GetByEmailAsync(string email);
    public Task<User?> GetByUsernameAsync(string username);
    public Task<bool> ExistsByEmailAsync(string email);
    public Task<bool> ExistsByUsernameAsync(string username);
    public Task AddAsync(User user);
    public void Delete(User user);
    public Task SaveChangesAsync();
}