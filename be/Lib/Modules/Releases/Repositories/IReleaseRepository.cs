using Lib.Modules.Releases.Entities;

namespace Lib.Modules.Releases.Repositories;

public interface IReleaseRepository
{
    public Task<List<Release>> GetAllAsync(ReleaseFilter filter);
    public Task<int> CountAsync(ReleaseFilter filter);
    public Task<Release?> GetLatestByProjectIdAsync(Guid projectId);
    public Task<Release?> GetByIdAsync(Guid id);
    public Task<ReleaseFile?> GetFileByIdAsync(Guid id);
    public Task AddAsync(Release release);
    public void Delete(Release release);
    public Task SaveChangesAsync();
}