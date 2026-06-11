using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Identities;
using Core.Storage;
using Lib.Modules.Projects.Services;
using Lib.Modules.Users.Services;
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
    IUserService userService,
    IProjectService projectService,
    IRefService refService,
    ICommitService commitService,
    IPushService pushService,
    IMergeService mergeService,
    IBlobMetadataRepository blobMetadataRepository
) : ControllerBase
{
    [HttpGet("{projectId:guid}/vcs/pull")]
    public async Task<ActionResult<PullResultBodyDto>> Pull(
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

        return Ok(new PullResultBodyDto(
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
        [FromBody] PushRequestBodyDto bodyDto,
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

        var result = await pushService.ApplyPushAsync(
            project,
            bodyDto.RefName,
            bodyDto.ExpectedHead,
            bodyDto.Commits.Select(c => c.ToDomain()),
            cancellationToken
        );

        if (result == PushResult.RefMismatch)
        {
            return Conflict(new
            {
                message = "Ref values mismatch detected"
            });
        }

        return NoContent();
    }

    [Authorize]
    [HttpPost("{projectId:guid}/vcs/merge")]
    public async Task<ActionResult> Merge(
        [FromRoute] Guid projectId,
        [FromBody] MergeRequestBodyDto bodyDto,
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
            return StatusCode(403, new { message = "You cannot perform merges in this project" });
        }

        if (bodyDto.TargetRefName == bodyDto.SourceRefName)
        {
            return BadRequest(new { message = "Cannot merge a branch into itself" });
        }

        var user = await userService.GetByIdAsync(userId);
        if (user is null)
        {
            return Unauthorized(new
            {
                message = "Unable to fetch user data"
            });
        }

        var result = await mergeService.MergeAsync(
            bodyDto.Title,
            projectId,
            bodyDto.TargetRefName,
            bodyDto.SourceRefName,
            bodyDto.ExpectedTargetHead,
            bodyDto.ExpectedSourceHead,
            new UserIdentity(
                user.Id,
                user.DisplayName,
                user.Email
            ),
            cancellationToken
        );

        return result switch
        {
            MergeResult.RefMismatch => Conflict(new { message = "Ref values mismatch detected" }),
            MergeResult.RefNotFound => NotFound(new { message = "Ref not found" }),
            MergeResult.RefValueNull => UnprocessableEntity(new { message = "Cannot perform merge on empty branches" }),
            MergeResult.Success => NoContent(),
            _ => throw new UnreachableException()
        };
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        return userIdClaim is null ? null : Guid.Parse(userIdClaim.Value);
    }
}