namespace Lib.Modules.Auth.DTOs;

public record TokenPairDto(
    string AccessToken,
    string RefreshToken
);