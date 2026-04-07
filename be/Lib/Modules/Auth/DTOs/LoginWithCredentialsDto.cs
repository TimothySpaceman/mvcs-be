namespace Lib.Modules.Auth.DTOs;

public record LoginWithCredentialsDto(
    string EmailOrUsername,
    string Password
);