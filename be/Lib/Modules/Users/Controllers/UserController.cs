using Lib.Modules.Users.DTOs;
using Lib.Modules.Users.Services;
using Lib.Shared.DTOs;
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
        [FromQuery] int itemsPerPage = DefaultItemsPerPage
    )
    {
        if (page < MinPage || itemsPerPage < MinItemsPerPage || itemsPerPage > MaxItemsPerPage)
        {
            return BadRequest(new { message = "Invalid pagination parameters" });
        }

        var result = await userService.GetAllAsync(page, itemsPerPage);
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
}