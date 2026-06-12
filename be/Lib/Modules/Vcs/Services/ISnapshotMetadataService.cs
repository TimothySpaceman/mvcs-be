using Core.Storage;
using Lib.Modules.Vcs.Entities;

namespace Lib.Modules.Vcs.Services;

public interface ISnapshotMetadataService
{
    public Task<SnapshotMetadata?> GetAsync(
        HashId commitId,
        Guid projectId,
        CancellationToken cancellationToken = default
    );

    public Task SubmitAsync(
        HashId commitId,
        Guid projectId,
        Dictionary<string, string[]> data,
        CancellationToken cancellationToken = default
    );
}