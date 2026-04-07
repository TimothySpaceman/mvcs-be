using Lib.Modules.Auth.Entities;

namespace Lib.Modules.Auth.Repositories;

public interface IUserCredentialsRepository
{
    public Task<UserCredentials?> GetByIdAsync(Guid id);
    public Task<UserCredentials?> GetByUserIdAsync(Guid userId);
    public Task<bool> ExistsByUserIdAsync(Guid userId);
    public Task AddAsync(UserCredentials credentials);
    public void Delete(UserCredentials credentials);
    public Task SaveChangesAsync();
}