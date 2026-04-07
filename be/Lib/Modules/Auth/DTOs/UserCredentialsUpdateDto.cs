namespace Lib.Modules.Auth.DTOs;

public record UserCredentialsUpdateDto(
    Guid UserId,
    string NewPassword
);