using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lib.Modules.Storages.DTOs;
using Lib.Modules.Storages.Entities;
using Lib.Modules.Storages.Services;
using Lib.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Storages.Controllers;

record Access(StorageAccess? AccessRecord, bool CanRead, bool CanWrite, bool IsOwner);

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
        var (storage, access) = await GetStorageWithAccess(id);
        return access.CanRead ? Ok(storage) : NotFound(new { message = "Storage not found" });
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
        var (_, access) = await GetStorageWithAccess(id);

        if (!access.CanRead) return NotFound();
        if (!access.IsOwner) return StatusCode(403, new { message = "You cannot edit this storage" });

        var updated = await storageService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var (_, access) = await GetStorageWithAccess(id);

        if (!access.CanRead) return NotFound();
        if (!access.IsOwner) return StatusCode(403, new { message = "You cannot delete this storage config" });

        await storageService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id:guid}/config")]
    public async Task<ActionResult<StorageConfigDto>> GetConfig(Guid id)
    {
        var (_, access) = await GetStorageWithAccess(id);

        if (!access.CanRead) return NotFound();
        if (!access.IsOwner) return StatusCode(403, new { message = "You cannot access this storage config" });

        var result = await storageService.GetConfigAsync(id);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/config")]
    public async Task<ActionResult> UpdateConfig(Guid id, [FromBody] StorageUpdateConfigDto dto)
    {
        var (_, access) = await GetStorageWithAccess(id);

        if (!access.CanRead) return NotFound();
        if (!access.IsOwner) return StatusCode(403, new { message = "You cannot edit this storage config" });

        await storageService.UpdateConfigAsync(id, dto);
        return NoContent();
    }

    [HttpPut("{id:guid}/access/{targetUserId:guid}")]
    public async Task<ActionResult> GrantAccess(Guid id, Guid targetUserId, [FromBody] StorageGrantAccessDto dto)
    {
        var (_, access) = await GetStorageWithAccess(id);

        if (!access.CanRead) return NotFound();
        if (!access.IsOwner) return StatusCode(403, new { message = "You cannot manage access for this project" });

        await storageService.GrantAccessAsync(id, dto with { UserId = targetUserId });
        return NoContent();
    }

    [HttpDelete("{id:guid}/access/{targetUserId:guid}")]
    public async Task<ActionResult> RevokeAccess(Guid id, Guid targetUserId)
    {
        var (_, access) = await GetStorageWithAccess(id);

        if (!access.CanRead) return NotFound();
        if (!access.IsOwner) return StatusCode(403, new { message = "You cannot manage access for this project" });

        await storageService.RevokeAccessAsync(id, targetUserId);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (claim is null) throw new UnauthorizedException("Unable to identify the user");
        return Guid.Parse(claim.Value);
    }

    private async Task<(Storage, Access)> GetStorageWithAccess(Guid storageId)
    {
        var userId = GetUserId();
        var storage = await storageService.GetRawByIdAsync(storageId);
        var accessRecord = storage.AccessEntries.FirstOrDefault(a => a.UserId == userId);

        var hasAccess = accessRecord is not null;
        var access = new Access(
            accessRecord,
            (hasAccess && accessRecord!.CanRead) || storage.IsPublic,
            hasAccess && accessRecord!.CanWrite,
            hasAccess && accessRecord!.IsOwner
        );

        return (storage, access);
    }
}