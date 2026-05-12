using Lib.Modules.Vcs.Entities;

namespace Lib.Modules.Vcs.Repository;

public interface IRefRepository
{
    public Task<RefEntity?> GetAsync(Guid projectId, string name, CancellationToken cancellationToken = default);
    public Task<List<RefEntity>> GetAllAsync(Guid projectId, CancellationToken cancellationToken = default);
    public Task AddAsync(RefEntity entry, CancellationToken cancellationToken = default);
    public void Delete(RefEntity entry);
    public Task SaveChangesAsync(CancellationToken cancellationToken = default);
}