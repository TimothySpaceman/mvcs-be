using Core.Commits;
using Core.Storage;

namespace Lib.Modules.Vcs.Repository;

public interface ICommitRepository
{
    public Task<bool> HasAsync(
        Guid projectId,
        HashId id,
        CancellationToken cancellationToken = default
    );

    public Task<Commit?> GetAsync(
        Guid projectId,
        HashId id,
        CancellationToken cancellationToken = default
    );

    public Task<Dictionary<HashId, Commit>> GetAllAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    );

    public Task AddAsync(
        Guid projectId,
        Commit commit,
        CancellationToken cancellationToken = default
    );

    public Task AddRangeAsync(
        Guid projectId,
        IEnumerable<Commit> commits,
        CancellationToken cancellationToken = default
    );

    public Task<bool> RemoveAsync(
        Guid projectId,
        HashId id,
        CancellationToken cancellationToken = default
    );

    public Task SaveChangesAsync(CancellationToken cancellationToken = default);
}