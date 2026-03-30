using Lib.Modules.Auth.Entities;

namespace Lib.Modules.Auth.Repositories;

public interface ISessionRepository
{
    public Task<Session?> GetByIdAsync(Guid id);
    public Task<List<Session>> GetByUserIdAsync(Guid userId);
    public Task<bool> ExistsByIdAsync(Guid id);
    public Task AddAsync(Session session);
    public void Delete(Session session);
    public Task SaveChangesAsync();
}