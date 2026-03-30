namespace Lib.Modules.Auth.DTOs;

public record RegisterDto(
    string Username,
    string DisplayName,
    string Email,
    string PlainPassword
);