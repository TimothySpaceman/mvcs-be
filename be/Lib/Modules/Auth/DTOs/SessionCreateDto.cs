using Lib.Modules.Auth.Entities;

namespace Lib.Modules.Auth.DTOs;

public record SessionCreateDto(
    Guid UserId,
    DeviceInfo DeviceInfo,
    string IpAddress
);