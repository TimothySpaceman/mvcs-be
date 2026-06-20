using Lib.Modules.Auth.Entities;

namespace Lib.Modules.Auth.Repositories;

public interface ISessionRepository
{
    public Task<Session?> GetByIdAsync(Guid id);
    public Task<List<Session>> GetByUserIdAsync(Guid userId);
    public Task<List<Session>> GetPageByUserIdAsync(Guid userId, Guid? beforeId, int limit);
    public Task<Session?> GetByTokenHashAsync(string tokenHash);
    public Task AddAsync(Session session);
    public void Delete(Session session);
    public Task SaveChangesAsync();
}