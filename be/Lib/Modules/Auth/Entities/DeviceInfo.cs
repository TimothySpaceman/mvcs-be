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

    public static DeviceInfo Create(
        string userAgent,
        string device,
        string os,
        string browser
    )
    {
        return new DeviceInfo
        {
            UserAgent = userAgent,
            Device = device,
            OS = os,
            Browser = browser
        };
    }
};