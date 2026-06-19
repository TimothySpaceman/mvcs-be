using Lib.Modules.Users.DTOs;

namespace Lib.Modules.Auth.DTOs;

public record TokenPairDto(
    string AccessToken,
    string RefreshToken,
    UserDto User,
    Guid SessionId
);