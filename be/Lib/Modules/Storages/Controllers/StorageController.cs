using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lib.Modules.Storages.DTOs;
using Lib.Modules.Storages.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Storages.Controllers;

[Authorize]
[ApiController]
[Route("api/storages")]
public class StorageController(IStorageService storageService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<StorageDto>>> GetAll()
    {
        var userId = GetUserId();
        var result = await storageService.GetAllByUserIdAsync(userId);
        return Ok(result);
    }
 
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StorageDto>> GetById(Guid id)
    {
        var userId = GetUserId();
        var result = await storageService.GetByIdAsync(id, userId);
        if (result is null) return NotFound();
        return Ok(result);
    }
 
    [HttpGet("{id:guid}/config")]
    public async Task<ActionResult<StorageConfigDto>> GetConfig(Guid id)
    {
        var userId = GetUserId();
        var result = await storageService.GetConfigAsync(id, userId);
        return Ok(result);
    }
 
    [HttpPost]
    public async Task<ActionResult<StorageDto>> Create([FromBody] StorageCreateDto dto)
    {
        var userId = GetUserId();
        var result = await storageService.CreateAsync(userId, dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
 
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<StorageDto>> Update(Guid id, [FromBody] StorageUpdateDto dto)
    {
        var userId = GetUserId();
        var result = await storageService.UpdateAsync(id, userId, dto);
        return Ok(result);
    }
 
    [HttpPatch("{id:guid}/config")]
    public async Task<IActionResult> UpdateConfig(Guid id, [FromBody] StorageUpdateConfigDto dto)
    {
        var userId = GetUserId();
        await storageService.UpdateConfigAsync(id, userId, dto);
        return NoContent();
    }
 
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        await storageService.DeleteAsync(id, userId);
        return NoContent();
    }
 
    [HttpPut("{id:guid}/access/{targetUserId:guid}")]
    public async Task<IActionResult> GrantAccess(Guid id, Guid targetUserId, [FromBody] StorageGrantAccessDto dto)
    {
        var userId = GetUserId();
        await storageService.GrantAccessAsync(id, userId, dto with { UserId = targetUserId });
        return NoContent();
    }
 
    [HttpDelete("{id:guid}/access/{targetUserId:guid}")]
    public async Task<IActionResult> RevokeAccess(Guid id, Guid targetUserId)
    {
        var userId = GetUserId();
        await storageService.RevokeAccessAsync(id, userId, targetUserId);
        return NoContent();
    }
 
    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
 
        if (claim is null) throw new UnauthorizedAccessException("Unable to identify the user");
        return Guid.Parse(claim.Value);
    }
}