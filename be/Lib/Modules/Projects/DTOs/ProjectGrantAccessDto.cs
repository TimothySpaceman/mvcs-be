using Lib.Modules.Projects.Entities;

namespace Lib.Modules.Projects.DTOs;

public record ProjectGrantAccessDto(Guid UserId, Entities.ProjectAccessType AccessType);