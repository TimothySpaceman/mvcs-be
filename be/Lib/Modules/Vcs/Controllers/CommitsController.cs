using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Commits;
using Lib.Modules.Projects.Services;
using Lib.Modules.Vcs.DTOs;
using Lib.Modules.Vcs.Helpers;
using Lib.Modules.Vcs.Services;
using Lib.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using ICommitService = Lib.Modules.Vcs.Services.ICommitService;

namespace Lib.Modules.Vcs.Controllers;

[ApiController]
[Route("api/projects")]
public class CommitsController(
    IProjectService projectService,
    IRefService refService,
    ICommitService commitService
) : ControllerBase
{
    private const int DefaultHistoryLimit = 50;
    private const int MaxHistoryLimit = 200;
    private const int MinHistoryLimit = 1;
    
    [HttpGet("{projectId:guid}/vcs/commits")]
    public async Task<ActionResult<CursorPagedResultDto<CommitInfoDto, string>>> GetHistory(
        [FromRoute] Guid projectId,
        [FromQuery] string refName,
        [FromQuery] string? fromId = null,
        [FromQuery] int limit = DefaultHistoryLimit,
        CancellationToken cancellationToken = default
    )
    {
        if (limit < MinHistoryLimit || limit > MaxHistoryLimit)
        {
            return BadRequest(new { message = "Invalid limit" });
        }

        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId();
        if (!project.CanRead(userId))
        {
            return NotFound(new { message = "Project not found" });
        }

        var refValue = await refService.GetRefValueAsync(projectId, refName, cancellationToken);
        if (refValue is null)
        {
            return NotFound(new { message = "Ref not found" });
        }

        var fromHashId = HashIdHelper.ParseNullable(fromId);

        var headId = fromHashId ?? refValue.Value;

        var chain = (await commitService.GetChainAsync(
            projectId, headId, null, limit + 1, cancellationToken
        )).ToList();

        var hasMore = chain.Count > limit;
        var page = chain.Take(limit).ToList();
        var nextCursor = hasMore ? chain[limit].Id.ToHexString() : null;

        return Ok(new CursorPagedResultDto<CommitInfoDto, string>(
            page.Select(CommitInfoDto.FromDomain),
            limit,
            nextCursor
        ));
    }
    
    [HttpGet("{projectId:guid}/vcs/commits/{commitId}/info")]
    public async Task<ActionResult<CommitInfoDto>> GetCommitInfo(
        [FromRoute] Guid projectId,
        [FromRoute] string commitId,
        CancellationToken cancellationToken = default
    )
    {
        var (commit, error) = await GetCommitOrError(projectId, commitId, cancellationToken);
        return error ?? Ok(CommitInfoDto.FromDomain(commit!));
    }

    [HttpGet("{projectId:guid}/vcs/commits/{commitId}")]
    public async Task<ActionResult<CommitDto>> GetCommitDetails(
        [FromRoute] Guid projectId,
        [FromRoute] string commitId,
        CancellationToken cancellationToken = default
    )
    {
        var (commit, error) = await GetCommitOrError(projectId, commitId, cancellationToken);
        return error ?? Ok(CommitDto.FromDomain(commit!));
    }
    
    private async Task<(Commit? commit, ObjectResult? error)> GetCommitOrError(
        Guid projectId,
        string commitId,
        CancellationToken cancellationToken
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        if (!project.CanRead(GetCurrentUserId()))
        {
            return (null, NotFound(new { message = "Project not found" }));
        }

        var hashId = HashIdHelper.ParseNullable(commitId);
        if (hashId is null)
        {
            return (null, BadRequest(new { message = "Invalid commit id" }));
        }

        var commit = await commitService.GetAsync(projectId, hashId.Value, cancellationToken);
        if (commit is null)
        {
            return (null, NotFound(new { message = "Commit not found" }));
        }

        return (commit, null);
    }
    
    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        return userIdClaim is null ? null : Guid.Parse(userIdClaim.Value);
    }
}