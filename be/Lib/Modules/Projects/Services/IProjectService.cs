using Lib.Modules.Projects.DTOs;
using Lib.Modules.Projects.Entities;
using Lib.Modules.Projects.Repositories;
using Lib.Shared.DTOs;

namespace Lib.Modules.Projects.Services;

public interface IProjectService
{
    public Task<PagedResultDto<ProjectDto>> SearchAsync(ProjectFilter filter, Guid? viewerUserId);
    public Task<List<ProjectDto>> GetAllByAuthorIdAsync(Guid authorId, Guid? userId = null);
    public Task<ProjectDto?> GetByIdAsync(Guid id, Guid? userId = null);
    public Task<Project> GetRawByIdAsync(Guid id);
    public Task<ProjectDto> CreateAsync(Guid authorId, ProjectCreateDto createDto);
    public Task<ProjectDto> UpdateAsync(Guid id, ProjectUpdateDto updateDto, Guid? userId = null);
    public Task InitializeAsync(Guid id, string defaultRefName);
    public Task InitializeAsync(Project project, string defaultRefName);
    public Task DeleteAsync(Guid id, bool soft = true);
    public Task GrantAccessAsync(Guid id, Guid targetUserId, ProjectAccessType accessType);
    public Task RevokeAccessAsync(Guid id, Guid targetUserId);
}