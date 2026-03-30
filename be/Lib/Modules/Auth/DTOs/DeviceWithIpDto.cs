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
        var (device, os, browser) = ParseUserAgent(userAgent);

        return new DeviceWithIpDto(
            ip,
            userAgent,
            device,
            os,
            browser
        );
    }

    private static (string device, string os, string browser) ParseUserAgent(string ua)
    {
        if (string.IsNullOrWhiteSpace(ua))
            return ("unknown", "unknown", "unknown");

        var uaLower = ua.ToLower();
        
        var os =
            uaLower.Contains("windows") ? "Windows" :
            uaLower.Contains("android") ? "Android" :
            uaLower.Contains("iphone") || uaLower.Contains("ipad") ? "iOS" :
            uaLower.Contains("mac os") || uaLower.Contains("macintosh") ? "macOS" :
            uaLower.Contains("linux") ? "Linux" :
            "unknown";
        
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
};