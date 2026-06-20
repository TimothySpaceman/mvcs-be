using Core.Exceptions;
using Core.Storage;
using Lib.Modules.Vcs.Entities;
using Lib.Modules.Vcs.Repository;

namespace Lib.Modules.Vcs.Services;

public class SnapshotMetadataService(
    ISnapshotMetadataRepository metadataRepository,
    ICommitRepository commitRepository
) : ISnapshotMetadataService
{
    public Task<SnapshotMetadata?> GetAsync(
        HashId commitId,
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        return metadataRepository.GetAsync(commitId, projectId, cancellationToken);
    }

    public async Task SubmitAsync(
        HashId commitId,
        Guid projectId,
        Dictionary<string, string[]> data,
        CancellationToken cancellationToken = default
    )
    {
        var commitExists = await commitRepository.HasAsync(projectId, commitId, cancellationToken);
        if (!commitExists)
        {
            throw new CommitNotFoundException($"Commit {commitId} not found");
        }

        var entity = SnapshotMetadata.Create(commitId, projectId, data, DateTimeOffset.UtcNow);
        await metadataRepository.UpsertAsync(entity, cancellationToken);
        await metadataRepository.SaveChangesAsync(cancellationToken);
    }
}