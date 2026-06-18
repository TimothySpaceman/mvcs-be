using Microsoft.AspNetCore.Http;

namespace Lib.Modules.Auth.DTOs;

public record DeviceWithIpDto(
    string Ip,
    string UserAgent,
    string Device,
    string OS,
    string Browser
)
{
    public static DeviceWithIpDto FromHttpContext(HttpContext context)
    {
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim()
                 ?? "unknown";

        var userAgent = context.Request.Headers["User-Agent"].ToString() ?? "unknown";
        var deviceName = context.Request.Headers["X-Device-Name"].FirstOrDefault();
        var (device, os, browser) = ParseUserAgent(userAgent);

        return new DeviceWithIpDto(
            ip,
            userAgent,
            deviceName ?? device,
            os,
            browser
        );
    }

    private static (string device, string os, string browser) ParseUserAgent(string ua)
    {
        if (string.IsNullOrWhiteSpace(ua))
            return ("unknown", "unknown", "unknown");

        var uaLower = ua.ToLower();

        var os = ParseOs(uaLower);

        var browser =
            uaLower.Contains("edg") ? "Edge" :
            uaLower.Contains("chrome") ? "Chrome" :
            uaLower.Contains("safari") && !uaLower.Contains("chrome") ? "Safari" :
            uaLower.Contains("firefox") ? "Firefox" :
            "unknown";

        var device =
            uaLower.Contains("mobile") ? "Mobile" :
            uaLower.Contains("tablet") ? "Tablet" :
            "Desktop";

        return (device, os, browser);
    }

    private static string ParseOs(string userAgent)
    {
        if (userAgent.Contains("maccatalyst") || userAgent.Contains("mac os x")) return "macOS";
        if (userAgent.Contains("windows") || userAgent.Contains("winui")) return "Windows";
        if (userAgent.Contains("linux")) return "Linux";
        if (userAgent.Contains("android")) return "Android";
        if (userAgent.Contains("ios")) return "iOS";
        return "unknown";
    }
};