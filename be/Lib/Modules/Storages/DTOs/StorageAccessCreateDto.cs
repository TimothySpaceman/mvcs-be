using Lib.Modules.Storages.Entities;

namespace Lib.Modules.Storages.DTOs;

public record StorageGrantAccessDto(Guid UserId, StorageAccessType AccessType);