using Lib.Modules.Projects.Entities;

namespace Lib.Modules.Projects.Repositories;

public interface IProjectRepository
{
    public Task<List<Project>> GetAllByAuthorIdAsync(Guid userId);
    public Task<List<Project>> GetAllByStorageIdAsync(Guid storageId);
    public Task<Project?> GetByIdAsync(Guid id);
    public Task<bool> ExistsByIdAsync(Guid id);
    public Task AddAsync(Project project);
    public void Delete(Project project);
    public Task SaveChangesAsync();
}