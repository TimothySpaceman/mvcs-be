namespace Lib.Modules.Auth.DTOs;

public record UserCredentialsVerifyDto(
    Guid UserId,
    string PlainPassword
);