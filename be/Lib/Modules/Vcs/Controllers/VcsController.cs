using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Storage;
using Lib.Modules.Projects.Services;
using Lib.Modules.Vcs.DTOs;
using Lib.Modules.Vcs.Helpers;
using Lib.Modules.Vcs.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Vcs.Controllers;

[ApiController]
[Route("api/projects")]
public class VcsController(
    IProjectService projectService,
    IRefService refService,
    ICommitService commitService
) : ControllerBase
{
    [HttpGet("{projectId:guid}/vcs/pull")]
    public async Task<ActionResult<PullResultDto>> Pull(
        [FromRoute] Guid projectId,
        [FromQuery] string refName,
        [FromQuery] string? fromId,
        CancellationToken cancellationToken
    )
    {
        var fromHashId = HashIdHelper.ParseNullable(fromId);

        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId();
        if (!project.CanRead(userId))
        {
            return NotFound(new
            {
                Message = "Project not found"
            });
        }

        var refValue = await refService.GetRefValueAsync(projectId, refName, cancellationToken);
        if (refValue is null)
        {
            return NotFound(new
            {
                Message = "Ref not found"
            });
        }

        var chain = await commitService.GetChainAsync(projectId, refValue.Value, fromHashId, cancellationToken);
        return Ok(new PullResultDto(chain.Select(CommitDto.FromDomain)));
    }

    [HttpGet("{projectId:guid}/vcs/snapshot")]
    public async Task<ActionResult<SnapshotDto>> GetSnapshot(
        [FromRoute] Guid projectId,
        [FromQuery] string? commitId,
        [FromQuery] string? refName,
        CancellationToken cancellationToken
    )
    {
        if (commitId is null && refName is null)
        {
            return BadRequest(new
            {
                Message = "Commit id or ref name is required"
            });
        }

        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId();
        if (!project.CanRead(userId))
        {
            return NotFound(new
            {
                Message = "Project not found"
            });
        }

        HashId targetId;
        if (commitId is not null)
        {
            targetId = HashIdHelper.Parse(commitId);
        }
        else
        {
            var refValue = await refService.GetRefValueAsync(projectId, refName!, cancellationToken);
            if (refValue is null)
            {
                return NotFound(new
                {
                    Message = "Ref not found"
                });
            }

            targetId = refValue.Value;
        }

        var snapshot = await commitService.GetSnapshotAsync(projectId, targetId, cancellationToken);
        return Ok(SnapshotDto.FromDomain(snapshot));
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        return userIdClaim is null ? null : Guid.Parse(userIdClaim.Value);
    }
}