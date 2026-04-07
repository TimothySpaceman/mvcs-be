namespace Lib.Modules.Auth.DTOs;

public record DeviceAuthStartDto(
    string UserCode,
    string DeviceCode,
    string VerificationUrl,
    DateTimeOffset ExpiresAt,
    double PollingIntervalSeconds
);