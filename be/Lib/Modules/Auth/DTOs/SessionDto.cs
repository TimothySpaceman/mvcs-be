using Lib.Modules.Auth.Entities;

namespace Lib.Modules.Auth.DTOs;

public record SessionDto(
    Guid Id,
    Guid UserId,
    DeviceInfo DeviceInfo,
    string IpAddress,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastActiveAt
)
{
    public static SessionDto FromSession(Session session)
    {
        return new SessionDto(
            session.Id,
            session.UserId,
            session.DeviceInfo,
            session.IpAddress,
            session.CreatedAt,
            session.LastActiveAt
        );
    }
};