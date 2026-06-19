using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lib.Modules.Auth.DTOs;
using Lib.Modules.Auth.Services;
using Lib.Shared.DTOs;
using Lib.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Auth.Controllers;

[Authorize]
[ApiController]
[Route("api/auth/sessions")]
public class SessionController(ISessionService sessionService) : ControllerBase
{
    private const int DefaultLimit = 20;
    private const int MaxLimit = 100;
    private const int MinLimit = 1;

    [HttpGet]
    public async Task<ActionResult<CursorPagedResultDto<SessionDto, Guid>>> GetAll(
        [FromQuery] Guid? cursor = null,
        [FromQuery] int limit = DefaultLimit
    )
    {
        if (limit is < MinLimit or > MaxLimit)
        {
            return BadRequest(new { message = "Invalid limit" });
        }

        var userId = GetCurrentUserId();
        var result = await sessionService.GetPageByUserIdAsync(userId, cursor, limit);
        return Ok(result);
    }

    [HttpDelete("{sessionId:guid}")]
    public async Task<ActionResult> RevokeById([FromRoute] Guid sessionId)
    {
        var userId = GetCurrentUserId();
        var revoked = await sessionService.RevokeByIdAsync(sessionId, userId);
        if (!revoked) return NotFound(new { message = "Session not found" });
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> RevokeAll()
    {
        var userId = GetCurrentUserId();
        await sessionService.RevokeAllByUserIdAsync(userId);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (claim is null) throw new UnauthorizedException("Unable to identify the user");
        return Guid.Parse(claim.Value);
    }
}