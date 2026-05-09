using Core.Storage;
using Lib.Modules.Vcs.Entities;

namespace Lib.Modules.Vcs.Repository;

public interface IBlobMetadataRepository
{
    public Task<BlobMetadataEntity?> GetByIdAsync(HashId id, Guid projectId);
    public Task<bool> ExistsByIdAsync(HashId id, Guid projectId);
    public Task<List<BlobMetadataEntity>> GetAllByProjectIdAsync(Guid projectId);
    public Task AddAsync(BlobMetadataEntity blob);
    public void Delete(BlobMetadataEntity blob);
    public Task SaveChangesAsync();
}