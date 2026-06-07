using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lib.Modules.Projects.Services;
using Lib.Modules.Vcs.DTOs;
using Lib.Modules.Vcs.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Vcs.Controllers;

[ApiController]
[Route("api/projects")]
public class RefsController(
    IProjectService projectService,
    IRefService refService
) : ControllerBase
{
    [HttpGet("{projectId:guid}/vcs/refs")]
    public async Task<ActionResult<IEnumerable<RefDto>>> GetRefs(
        [FromRoute] Guid projectId,
        CancellationToken cancellationToken
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId();
        if (!project.CanRead(userId))
        {
            return NotFound(new { message = "Project not found" });
        }

        var refs = await refService.GetAllRefsAsync(projectId, cancellationToken);

        return Ok(refs.Select(RefDto.FromEntity));
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        return userIdClaim is null ? null : Guid.Parse(userIdClaim.Value);
    }
}