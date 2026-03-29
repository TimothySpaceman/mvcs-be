namespace Lib.Modules.Auth.Entities;

public record DeviceInfo
{
    public string? UserAgent { get; init; }
    public string? Device { get; init; }
    public string? OS { get; init; }
    public string? Browser { get; init; }

    private DeviceInfo()
    {
    }
};