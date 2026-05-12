using Core.Commits;
using Core.Snapshots;
using Core.Storage;

namespace Lib.Modules.Vcs.Services;

public interface ICommitService
{
    public Task<Dictionary<HashId, Commit>> GetAllAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    );

    public Task<Commit?> GetAsync(
        Guid projectId,
        HashId commitId,
        CancellationToken cancellationToken = default
    );

    public IAsyncEnumerable<Commit> GetChainAsync(
        Guid projectId,
        HashId toId,
        HashId? fromId = null,
        CancellationToken cancellationToken = default
    );

    public Task<Snapshot> GetSnapshotAsync(
        Guid projectId,
        HashId commitId,
        CancellationToken cancellationToken = default
    );
}