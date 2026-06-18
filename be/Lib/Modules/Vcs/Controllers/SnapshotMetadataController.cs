using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Exceptions;
using Lib.Modules.Projects.Services;
using Lib.Modules.Vcs.DTOs;
using Lib.Modules.Vcs.Helpers;
using Lib.Modules.Vcs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Vcs.Controllers;

[ApiController]
[Route("api/projects")]
public class SnapshotMetadataController(
    IProjectService projectService,
    ISnapshotMetadataService snapshotMetadataService
) : ControllerBase
{
    [HttpGet("{projectId:guid}/vcs/commits/{commitId}/metadata")]
    public async Task<ActionResult<SnapshotMetadataDto>> GetMetadata(
        [FromRoute] Guid projectId,
        [FromRoute] string commitId,
        CancellationToken cancellationToken = default
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        if (!project.CanRead(GetCurrentUserId()))
        {
            return NotFound(new { message = "Project not found" });
        }

        var commitIdValue = HashIdHelper.ParseNullable(commitId);
        if (commitIdValue is null)
        {
            return BadRequest(new { message = "Invalid commit id" });
        }

        var metadata = await snapshotMetadataService.GetAsync(commitIdValue.Value, projectId, cancellationToken);
        if (metadata is null)
        {
            return NotFound(new { message = "Metadata not found" });
        }

        return Ok(SnapshotMetadataDto.FromEntity(metadata));
    }

    [Authorize]
    [HttpPost("{projectId:guid}/vcs/commits/{commitId}/metadata")]
    public async Task<ActionResult> SubmitMetadata(
        [FromRoute] Guid projectId,
        [FromRoute] string commitId,
        [FromBody] SubmitSnapshotMetadataDto bodyDto,
        CancellationToken cancellationToken = default
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId()!.Value;;
        if (!project.CanRead(userId))
        {
            return NotFound(new { message = "Project not found" });
        }
        
        if (!project.CanWrite(userId))
        {
            return StatusCode(403, new { message = "You cannot write metadata for this project" });
        }

        var commitIdValue = HashIdHelper.ParseNullable(commitId);
        if (commitIdValue is null)
        {
            return BadRequest(new { message = "Invalid commit id" });
        }

        try
        {
            await snapshotMetadataService.SubmitAsync(commitIdValue.Value, projectId, bodyDto.Data, cancellationToken);
        }
        catch (CommitNotFoundException)
        {
            return NotFound(new { message = "Commit not found" });
        }

        return NoContent();
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        return userIdClaim is null ? null : Guid.Parse(userIdClaim.Value);
    }
}