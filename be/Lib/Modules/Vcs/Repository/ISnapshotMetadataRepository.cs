using Core.Storage;
using Lib.Modules.Vcs.Entities;

namespace Lib.Modules.Vcs.Repository;

public interface ISnapshotMetadataRepository
{
    public Task<SnapshotMetadata?> GetAsync(
        HashId commitId,
        Guid projectId,
        CancellationToken cancellationToken = default
    );

    public Task UpsertAsync(
        SnapshotMetadata entity,
        CancellationToken cancellationToken = default
    );

    public Task SaveChangesAsync(CancellationToken cancellationToken = default);
}