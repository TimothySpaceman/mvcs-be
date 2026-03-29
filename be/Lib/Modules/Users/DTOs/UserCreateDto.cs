namespace Lib.Modules.Users.DTOs;

public record UserCreateDto(
    string Username,
    string DisplayName,
    string Email
);