using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lib.Modules.Projects.Services;
using Lib.Modules.Vcs.Services;
using Lib.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using tusdotnet.Models;

namespace Lib.Modules.Vcs.Endpoints;

public class BlobTusEndpoints
{
    public static async Task<DefaultTusConfiguration?> BlobUploads(HttpContext httpContext)
    {
        var projectId = GetProjectId(httpContext);
        var userId = GetUserId(httpContext);
        if (projectId is null || userId is null) return null;

        var projectService = httpContext.RequestServices.GetRequiredService<IProjectService>();
        try
        {
            var project = await projectService.GetRawByIdAsync(projectId.Value);
            if (!project.CanWrite(userId.Value)) return null;
        }
        catch (NotFoundException)
        {
            return null;
        }

        var blobTransferService = httpContext.RequestServices.GetRequiredService<IBlobTransferService>();
        return await blobTransferService.GetTusConfigurationAsync(projectId.Value, userId.Value);
    }

    private static Guid? GetProjectId(HttpContext httpContext)
    {
        var projectIdStr = httpContext.GetRouteValue("projectId")?.ToString();
        return Guid.TryParse(projectIdStr, out var projectId) ? projectId : null;
    }

    private static Guid? GetUserId(HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier) ??
                          httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub);

        return userIdClaim is null ? null : Guid.Parse(userIdClaim.Value);
    }
}