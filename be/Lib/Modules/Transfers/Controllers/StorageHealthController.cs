using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lib.Modules.Storages.Services;
using Lib.Modules.Transfers.DTOs;
using Lib.Modules.Transfers.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Transfers.Controllers;

[ApiController]
[Route("api/storages")]
public class StorageHealthController(
    IStorageService storageService,
    ITransferService transferService
) : ControllerBase
{
    [HttpGet("{storageId:guid}/health")]
    public async Task<ActionResult<StorageHealthDto>> GetStorageHealth(
        [FromRoute] Guid storageId,
        CancellationToken cancellationToken
    )
    {
        var storage = await storageService.GetRawByIdAsync(storageId);
        var userId = GetCurrentUserId();

        if (!storage.CanRead(userId))
        {
            return NotFound(new
            {
                message = "Storage not found"
            });
        }

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