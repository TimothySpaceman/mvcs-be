using Lib.Modules.Projects.Entities;

namespace Lib.Modules.Projects.DTOs;

public record ProjectDto(
    Guid Id,
    Guid AuthorId,
    string Title,
    string Description,
    bool IsPublic,
    bool IsInitialized,
    string? DefaultRefName,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    ProjectAccessLevel AccessLevel,
    Guid? StorageId
)
{
    public static ProjectDto FromProject(Project project, Guid? userId = null)
    {
        ProjectAccessLevel accessLevel = ProjectAccessLevel.Public;
        Guid? storageId = null;
        if (userId is not null)
        {
            if (project.AuthorId == userId)
            {
                accessLevel = ProjectAccessLevel.Owner;
                storageId = project.StorageId;
            }
            else
            {
                var entry = project.AccessEntries.FirstOrDefault(a => a.UserId == userId);
                if (entry is not null) accessLevel = entry.CanWrite ? ProjectAccessLevel.Write : ProjectAccessLevel.Read;
            }
        }
        
        return new ProjectDto(
            project.Id,
            project.AuthorId,
            project.Title,
            project.Description,
            project.IsPublic,
            project.IsInitialized,
            project.DefaultRefName,
            project.CreatedAt,
            project.UpdatedAt,
            accessLevel,
            storageId
        );
    }
}