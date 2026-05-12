using Lib.Modules.Projects.DTOs;
using Lib.Modules.Projects.Entities;

namespace Lib.Modules.Projects.Services;

public interface IProjectService
{
    public Task<List<ProjectDto>> GetAllByAuthorIdAsync(Guid authorId);
    public Task<ProjectDto?> GetByIdAsync(Guid id);
    public Task<Project> GetRawByIdAsync(Guid id);
    public Task<ProjectDto> CreateAsync(Guid authorId, ProjectCreateDto createDto);
    public Task<ProjectDto> UpdateAsync(Guid id, ProjectUpdateDto updateDto);
    public Task InitializeAsync(Guid id);
    public Task InitializeAsync(Project project);
    public Task DeleteAsync(Guid id, bool soft = true);
    public Task GrantAccessAsync(Guid id, ProjectGrantAccessDto grantDto);
    public Task RevokeAccessAsync(Guid id, Guid targetUserId);
}