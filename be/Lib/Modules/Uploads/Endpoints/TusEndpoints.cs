using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lib.Modules.Uploads.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using tusdotnet.Models;

namespace Lib.Modules.Uploads.Endpoints;

public static class TusEndpoints
{
    public static async Task<DefaultTusConfiguration?> StorageUploads(HttpContext httpContext)
    {
        var storageId = GetStorageId(httpContext);
        var userId = GetUserId(httpContext);
 
        if (storageId is null || userId is null) return null;
 
        var uploadService = httpContext.RequestServices.GetRequiredService<IUploadService>();
        return await uploadService.GetTusConfigurationAsync(
            (Guid)storageId,
            (Guid)userId,
            scopePath: "files",
            httpContext
        );
    }

    private static Guid? GetStorageId(HttpContext httpContext)
    {
        var storageIdStr = httpContext.GetRouteValue("storageId")?.ToString();
        if (!Guid.TryParse(storageIdStr, out var storageId))
        {
            return null;
        }

        return storageId;
    }

    private static Guid? GetUserId(HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier) ??
                          httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub);

        return userIdClaim is null ? null : Guid.Parse(userIdClaim.Value);
    }
}