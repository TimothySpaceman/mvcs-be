using Lib.Modules.Users.DTOs;

namespace Lib.Modules.Auth.DTOs;

public record UserMetadataDto(
    Guid Id,
    string Username,
    string DisplayName,
    bool IsEmailVerified,
    UserAvatarDto? Avatar,
    Guid SessionId
)
{
    public static UserMetadataDto FromUserDto(UserDto userDto, Guid sessionId)
    {
        return new UserMetadataDto(
            userDto.Id,
            userDto.Username,
            userDto.DisplayName,
            userDto.IsEmailVerified,
            userDto.Avatar,
            sessionId
        );
    }
};