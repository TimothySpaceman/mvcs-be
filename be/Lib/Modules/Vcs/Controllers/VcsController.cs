using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Storage;
using Lib.Modules.Projects.Services;
using Lib.Modules.Vcs.DTOs;
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
        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId();
        if (!project.CanRead(userId))
        {
            return NotFound(new
            {
                Message = "Project not found"
            });
        }

        HashId? fromHashId = null;
        if (fromId is not null)
        {
            try
            {
                fromHashId = new HashId(Convert.FromHexString(fromId));
            }
            catch (FormatException)
            {
                return BadRequest(new
                {
                    Message = "Provided starting id is not valid hex string"
                });
            }
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

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        return userIdClaim is null ? null : Guid.Parse(userIdClaim.Value);
    }
}