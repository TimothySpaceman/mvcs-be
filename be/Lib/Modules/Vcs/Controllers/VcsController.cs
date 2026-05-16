using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Storage;
using Lib.Modules.Projects.Services;
using Lib.Modules.Vcs.DTOs;
using Lib.Modules.Vcs.Helpers;
using Lib.Modules.Vcs.Repository;
using Lib.Modules.Vcs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Vcs.Controllers;

[ApiController]
[Route("api/projects")]
public class VcsController(
    IProjectService projectService,
    IRefService refService,
    ICommitService commitService,
    IPushService pushService,
    IBlobMetadataRepository blobMetadataRepository
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
                message = "Project not found"
            });
        }

        var refValue = await refService.GetRefValueAsync(projectId, refName, cancellationToken);
        if (refValue is null)
        {
            return NotFound(new
            {
                message = "Ref not found"
            });
        }

        var chain = await commitService.GetChainAsync(projectId, refValue.Value, fromHashId, cancellationToken);
        var chainList = chain.ToList();
        var blobIds = BlobHelper.GetBlobsFromCommitsChain(chainList);
        var blobs = await blobMetadataRepository.GetAllByIdsAsync(blobIds, projectId, cancellationToken);

        return Ok(new PullResultDto(
            chainList.Select(CommitDto.FromDomain),
            blobs.Select(BlobMetadataDto.FromDomain)
        ));
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
                message = "Commit id or ref name is required"
            });
        }

        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId();
        if (!project.CanRead(userId))
        {
            return NotFound(new
            {
                message = "Project not found"
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
                    message = "Ref not found"
                });
            }

            targetId = refValue.Value;
        }

        var snapshot = await commitService.GetSnapshotAsync(projectId, targetId, cancellationToken);
        return Ok(SnapshotDto.FromDomain(snapshot));
    }

    [Authorize]
    [HttpPost("{projectId:guid}/vcs/push")]
    public async Task<ActionResult> Push(
        [FromRoute] Guid projectId,
        [FromBody] PushRequestDto bodyDto,
        CancellationToken cancellationToken
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId()!.Value;
        if (!project.CanRead(userId))
        {
            return NotFound(new
            {
                message = "Project not found"
            });
        }

        if (!project.CanWrite(userId))
        {
            return StatusCode(403, new { message = "You cannot push to this project" });
        }

        var refValue = await refService.GetRefValueAsync(projectId, bodyDto.RefName, cancellationToken);
        if (refValue != HashIdHelper.ParseNullable(bodyDto.ExpectedHead))
        {
            return Conflict(new
            {
                message = "Ref values mismatch detected"
            });
        }

        await pushService.UpdateCommitsChainAsync(
            project,
            bodyDto.RefName,
            bodyDto.Commits.Select(c => c.ToDomain()),
            cancellationToken
        );

        return NoContent();
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        return userIdClaim is null ? null : Guid.Parse(userIdClaim.Value);
    }
}