using Lib.Modules.Projects.Entities;

namespace Lib.Modules.Projects.DTOs;

public record ProjectDto(
    Guid Id,
    Guid AuthorId,
    string Title,
    string Description,
    bool IsPublic,
    bool IsInitialized,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
)
{
    public static ProjectDto FromProject(Project project)
    {
        return new ProjectDto(
            project.Id,
            project.AuthorId,
            project.Title,
            project.Description,
            project.IsPublic,
            project.IsInitialized,
            project.CreatedAt,
            project.UpdatedAt
        );
    }
};