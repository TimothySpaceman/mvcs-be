using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lib.Modules.Projects.Services;
using Lib.Modules.Releases.DTOs;
using Lib.Modules.Releases.Repositories;
using Lib.Modules.Releases.Services;
using Lib.Modules.Transfers.Adapters;
using Lib.Modules.Vcs.Helpers;
using Lib.Modules.Vcs.Services;
using Lib.Shared.DTOs;
using Lib.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Releases.Controllers;

[ApiController]
[Route("api/projects")]
public class ReleaseController(
    IProjectService projectService,
    IReleaseService releaseService,
    IBlobTransferService blobTransferService
) : ControllerBase
{
    private const int MaxItemsPerPage = 100;
    private const int MinItemsPerPage = 1;
    private const int DefaultItemsPerPage = 20;
    private const int MinPage = 1;

    [HttpGet("{projectId:guid}/releases")]
    public async Task<ActionResult<PagedResultDto<ReleaseDto>>> GetAll(
        [FromRoute] Guid projectId,
        [FromQuery] int page = MinPage,
        [FromQuery] int itemsPerPage = DefaultItemsPerPage
    )
    {
        if (page < MinPage || itemsPerPage < MinItemsPerPage || itemsPerPage > MaxItemsPerPage)
        {
            return BadRequest(new { message = "Invalid pagination parameters" });
        }

        var project = await projectService.GetRawByIdAsync(projectId);
        if (!project.CanRead(GetCurrentUserId(allowAnonymous: true)))
        {
            return NotFound(new { message = "Project not found" });
        }

        var releases = await releaseService.GetAllAsync(new ReleaseFilter
        {
            ProjectId = projectId,
            Page = page,
            ItemsPerPage = itemsPerPage
        });

        return Ok(releases);
    }

    [HttpGet("{projectId:guid}/releases/latest")]
    public async Task<ActionResult<ReleaseDto>> GetLatest(
        [FromRoute] Guid projectId
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        if (!project.CanRead(GetCurrentUserId(allowAnonymous: true)))
        {
            return NotFound(new { message = "Project not found" });
        }

        var release = await releaseService.GetLatestAsync(projectId);
        if (release is null)
        {
            return NotFound(new { message = "Release not found" });
        }

        return Ok(release);
    }

    [HttpGet("{projectId:guid}/releases/files/{fileId:guid}")]
    public async Task DownloadFileAsync(
        [FromRoute] Guid projectId,
        [FromRoute] Guid fileId,
        [FromQuery] string? fileName,
        CancellationToken cancellationToken
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        if (!project.CanRead(GetCurrentUserId(allowAnonymous: true)))
        {
            throw new NotFoundException("Project not found");
        }

        var file = await releaseService.GetRawFileByIdAsync(fileId);
        var release = await releaseService.GetRawByIdAsync(file.ReleaseId);
        if (release.ProjectId != projectId)
        {
            throw new NotFoundException("Release file not found");
        }

        var blobHashId = HashIdHelper.Parse(file.BlobId);

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

            var downloadName = string.IsNullOrWhiteSpace(fileName) ? file.FileName : fileName;
            var encodedFileName = Uri.EscapeDataString(downloadName);
            Response.Headers.Append(
                "Content-Disposition",
                $"attachment; filename*=UTF-8''{encodedFileName}"
            );

            await stream.CopyToAsync(Response.Body, cancellationToken);
        }
    }

    [Authorize]
    [HttpPost("{projectId:guid}/releases")]
    public async Task<ActionResult<ReleaseDto>> Create(
        [FromRoute] Guid projectId,
        [FromBody] CreateReleaseDto dto
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId();
        if (!project.CanReadExplicitly(userId))
        {
            return NotFound(new { message = "Project not found" });
        }
        if (!project.CanWrite(userId))
        {
            return StatusCode(403, new { message = "You cannot manage releases for this project" });
        }

        var release = await releaseService.CreateAsync(projectId, userId, dto);
        return Ok(release);
    }

    [Authorize]
    [HttpDelete("{projectId:guid}/releases/{releaseId:guid}")]
    public async Task<ActionResult> Delete(
        [FromRoute] Guid projectId,
        [FromRoute] Guid releaseId
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId();
        if (!project.CanReadExplicitly(userId))
        {
            return NotFound(new { message = "Project not found" });
        }

        var release = await releaseService.GetRawByIdAsync(releaseId);
        if (release.ProjectId != projectId)
        {
            return NotFound(new { message = "Release not found" });
        }

        var isOwner = project.AuthorId == userId;
        var isAuthor = release.AuthorId == userId;
        if (!isOwner && !(project.CanWrite(userId) && isAuthor))
        {
            return StatusCode(403, new { message = "You cannot delete this release" });
        }

        await releaseService.DeleteAsync(releaseId);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        return (Guid)GetCurrentUserId(false)!;
    }

    private Guid? GetCurrentUserId(bool allowAnonymous)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (claim is null && !allowAnonymous) throw new UnauthorizedException("Unable to identify the user");
        return claim is null ? null : Guid.Parse(claim.Value);
    }
}