using Lib.Modules.Projects.DTOs;
using Lib.Modules.Projects.Entities;
using Lib.Modules.Projects.Repositories;
using Lib.Shared.Exceptions;

namespace Lib.Modules.Projects.Services;

public class ProjectService(IProjectRepository repository) : IProjectService
{
    public async Task<List<ProjectDto>> GetAllByAuthorIdAsync(Guid authorId)
    {
        var projects = await repository.GetAllByAuthorIdAsync(authorId);
        return projects.Select(ProjectDto.FromProject).ToList();
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        var project = await repository.GetByIdAsync(id);
        return project is null ? null : ProjectDto.FromProject(project);
    }

    public async Task<Project> GetRawByIdAsync(Guid id)
    {
        var project = await repository.GetByIdAsync(id);
        if (project is null) throw new NotFoundException("Project not found");
        return project;
    }

    public async Task<ProjectDto> CreateAsync(Guid authorId, ProjectCreateDto createDto)
    {
        var project = Project.Create(
            authorId,
            createDto.StorageId,
            createDto.Title,
            createDto.Description,
            createDto.IsPublic
        );
        await repository.AddAsync(project);
        await repository.SaveChangesAsync();
        return ProjectDto.FromProject(project);
    }

    public async Task<ProjectDto> UpdateAsync(Guid id, ProjectUpdateDto updateDto)
    {
        var project = await GetRawByIdAsync(id);
        if (updateDto.Title is not null) project.Rename(updateDto.Title);
        if (updateDto.Description is not null) project.UpdateDescription(updateDto.Description);
        if (updateDto.IsPublic is not null) project.UpdateVisibility(updateDto.IsPublic.Value);
        if (updateDto.DefaultRefName is not null) project.UpdateDefaultRef(updateDto.DefaultRefName);
        await repository.SaveChangesAsync();
        return ProjectDto.FromProject(project);
    }

    public async Task InitializeAsync(Project project, string defaultRefName)
    {
        project.Initialize(defaultRefName);
        await repository.SaveChangesAsync();
    }

    public async Task InitializeAsync(Guid id, string defaultRefName)
    {
        var project = await GetRawByIdAsync(id);
        await InitializeAsync(project, defaultRefName);
    }

    public async Task DeleteAsync(Guid id, bool soft = true)
    {
        var project = await GetRawByIdAsync(id);
        if (soft) project.Delete();
        else repository.Delete(project);
        await repository.SaveChangesAsync();
    }

    public async Task GrantAccessAsync(Guid id, ProjectGrantAccessDto grantDto)
    {
        var project = await GetRawByIdAsync(id);

        var existing = project.AccessEntries.FirstOrDefault(a => a.UserId == grantDto.UserId);
        if (existing is not null)
        {
            existing.ChangeAccessType(grantDto.AccessType);
        }
        else
        {
            var newAccess = ProjectAccess.Create(project.Id, grantDto.UserId, grantDto.AccessType);
            await repository.AddAccessAsync(newAccess);
        }

        await repository.SaveChangesAsync();
    }

    public async Task RevokeAccessAsync(Guid id, Guid targetUserId)
    {
        var project = await GetRawByIdAsync(id);

        var access = project.AccessEntries.FirstOrDefault(a => a.UserId == targetUserId);
        if (access is null)
        {
            throw new NotFoundException("Access entry not found");
        }

        repository.DeleteAccess(access);
        await repository.SaveChangesAsync();
    }
}