using Core.Blobs;
using Core.Storage;

namespace Lib.Modules.Vcs.Repository;

public interface IBlobMetadataRepository
{
    public Task<BlobMetadata?> GetByIdAsync(
        HashId id,
        Guid projectId,
        CancellationToken cancellationToken = default
    );

    public Task<bool> ExistsByIdAsync(
        HashId id,
        Guid projectId,
        CancellationToken cancellationToken = default
    );

    public Task<bool> AllExistAsync(
        IEnumerable<HashId> ids,
        Guid projectId,
        CancellationToken cancellationToken = default
    );

    public Task<List<BlobMetadata>> GetAllByIdsAsync(
        IEnumerable<HashId> ids,
        Guid projectId,
        CancellationToken cancellationToken = default
    );

    public Task<List<BlobMetadata>> GetAllByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    );

    public Task AddAsync(
        BlobMetadata blob,
        Guid projectId,
        CancellationToken cancellationToken = default
    );

    public Task DeleteAsync(
        HashId id,
        Guid projectId,
        CancellationToken cancellationToken = default
    );

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default
    );
}