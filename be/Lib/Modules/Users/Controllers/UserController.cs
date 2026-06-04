using Lib.Modules.Users.DTOs;
using Lib.Modules.Users.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Users.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await userService.GetByIdAsync(id);
        return user is not null ? Ok(user) : NotFound(new { message = "User not found" });
    }
}