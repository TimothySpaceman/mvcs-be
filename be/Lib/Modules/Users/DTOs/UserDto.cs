using Lib.Modules.Users.Entities;

namespace Lib.Modules.Users.DTOs;

public record UserDto(
    Guid Id,
    string Username,
    string DisplayName,
    string Email,
    bool IsEmailVerified,
    UserAvatarDto? Avatar,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
)
{
    public static UserDto FromUser(User user)
    {
        var avatarDto = user.Avatar is null ? null : UserAvatarDto.FromUserAvatar(user.Avatar);
        return new UserDto(
            user.Id,
            user.Username,
            user.DisplayName,
            user.Email,
            user.IsEmailVerified,
            avatarDto,
            user.CreatedAt,
            user.UpdatedAt
        );
    }
};