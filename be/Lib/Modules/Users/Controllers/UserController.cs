using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lib.Modules.Users.DTOs;
using Lib.Modules.Users.Repositories;
using Lib.Modules.Users.Services;
using Lib.Shared.DTOs;
using Lib.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Users.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IUserService userService) : ControllerBase
{
    private const int MaxItemsPerPage = 100;
    private const int MinItemsPerPage = 1;
    private const int DefaultItemsPerPage = 20;
    private const int MaxBulkIds = 100;
    private const int MinPage = 1;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<UserDto>>> GetAll(
        [FromQuery] int page = MinPage,
        [FromQuery] int itemsPerPage = DefaultItemsPerPage,
        [FromQuery] string? search = null
    )
    {
        if (page < MinPage || itemsPerPage < MinItemsPerPage || itemsPerPage > MaxItemsPerPage)
            return BadRequest(new { message = "Invalid pagination parameters" });

        var result = await userService.GetAllAsync(new UserFilter
        {
            Page = page,
            ItemsPerPage = itemsPerPage,
            Search = search
        });
        return Ok(result);
    }

    [HttpGet("bulk")]
    public async Task<ActionResult<List<UserDto>>> GetBulk([FromQuery] List<Guid> ids)
    {
        if (ids.Count == 0)
        {
            return BadRequest(new { message = "At least one id must be provided" });
        }

        if (ids.Count > MaxBulkIds)
        {
            return BadRequest(new { message = $"Bulk request cannot exceed {MaxBulkIds} ids" });
        }

        var users = await userService.GetAllByIdsAsync(ids);
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await userService.GetByIdAsync(id);
        return user is not null ? Ok(user) : NotFound(new { message = "User not found" });
    }
    
    [Authorize]
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UserUpdateDto dto)
    {
        if (id != GetCurrentUserId())
        {
            return StatusCode(403, new { message = "You cannot edit this user" });
        }

        var updated = await userService.UpdateByIdAsync(id, dto);
        return Ok(updated);
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (claim is null) throw new UnauthorizedException("Unable to identify the user");
        return Guid.Parse(claim.Value);
    }
}