using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lib.Modules.Projects.Services;
using Lib.Modules.Storages.Services;
using Lib.Modules.Transfers.DTOs;
using Lib.Modules.Transfers.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Vcs.Controllers;

[ApiController]
[Route("api/projects")]
public class HealthController(
    IProjectService projectService,
    IStorageService storageService,
    ITransferService transferService
) : ControllerBase
{
    [HttpGet("{projectId:guid}/health")]
    public async Task<ActionResult<StorageHealthDto>> GetStorageHealth(
        [FromRoute] Guid projectId,
        CancellationToken cancellationToken
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId();
        if (!project.CanRead(userId))
        {
            return NotFound(new
            {
                message = "Project not found"
            });
        }
        
        var storage = await storageService.GetRawByIdAsync(project.StorageId);

        var health = await transferService.GetStorageHealthAsync(storage, cancellationToken);
        return Ok(health);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        return userIdClaim is null ? null : Guid.Parse(userIdClaim.Value);
    }
}