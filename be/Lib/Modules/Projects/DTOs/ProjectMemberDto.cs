namespace Lib.Modules.Projects.DTOs;

public record ProjectMemberDto(Guid UserId, ProjectAccessLevel AccessLevel);