using Lib.Modules.Users.Entities;

namespace Lib.Modules.Users.DTOs;

public record UserAvatarDto(
    Guid Id,
    string Url,
    long SizeBytes,
    string MimeType,
    DateTimeOffset CreatedAt
)
{
    public static UserAvatarDto FromUserAvatar(UserAvatar userAvatar)
    {
        return new UserAvatarDto(
            userAvatar.Id,
            userAvatar.Url,
            userAvatar.SizeBytes,
            userAvatar.MimeType,
            userAvatar.CreatedAt
        );
    }
};