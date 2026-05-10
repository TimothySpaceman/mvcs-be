using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Storage;
using Lib.Modules.Projects.Services;
using Lib.Modules.Transfers.Adapters;
using Lib.Modules.Vcs.Services;
using Lib.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using tusdotnet.Models;

namespace Lib.Modules.Vcs.Controllers;

[ApiController]
[Route("api/projects")]
public class BlobController(
    IProjectService projectService,
    IBlobTransferService blobTransferService
) : ControllerBase
{
    [HttpGet("{projectId:guid}/blobs/{blobId}")]
    public async Task DownloadFileAsync(
        [FromRoute] Guid projectId,
        [FromRoute] string blobId,
        CancellationToken cancellationToken
    )
    {
        HashId blobHashId;
        try
        {
            blobHashId = new HashId(Convert.FromHexString(blobId));
        }
        catch (FormatException ex)
        {
            throw new BadRequestException("Blob id is not valid hex string", ex);
        }

        var userId = GetCurrentUserId();
        var project = await projectService.GetRawByIdAsync(projectId);
        if (!project.CanRead(userId)) throw new NotFoundException("Project not found");

        ByteRange? range = null;
        if (Request.Headers.TryGetValue("Range", out var rangeHeader))
        {
            range = ByteRange.FromHeaderValue(rangeHeader!);
        }

        var (stream, totalLength, clampedRange) = await blobTransferService.GetBlobAsync(
            project,
            blobHashId,
            range,
            cancellationToken
        );
        var responseLength = clampedRange.End - clampedRange.Start + 1;

        await using (stream)
        {
            var bufferingFeature = HttpContext.Features.Get<IHttpResponseBodyFeature>();
            bufferingFeature?.DisableBuffering();

            Response.StatusCode = range is null ? 200 : 206;
            Response.Headers.Append("Accept-Ranges", "bytes");
            Response.Headers.Append("Content-Type", "application/octet-stream");
            Response.Headers.Append("Content-Length", responseLength.ToString());
            if (range is not null)
            {
                Response.Headers.Append(
                    "Content-Range",
                    $"bytes {clampedRange.Start}-{clampedRange.End}/{totalLength}"
                );
            }

            await stream.CopyToAsync(Response.Body, cancellationToken);
        }
    }

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
            var blobTransferService = httpContext.RequestServices.GetRequiredService<IBlobTransferService>();
            return await blobTransferService.GetTusConfigurationAsync(project, userId.Value);
        }
        catch (NotFoundException)
        {
            return null;
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        return userIdClaim is null ? null : Guid.Parse(userIdClaim.Value);
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