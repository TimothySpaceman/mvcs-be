namespace Lib.Modules.Auth.DTOs;

public record UserCredentialsCreateDto(
    Guid UserId,
    string PlainPassword
);