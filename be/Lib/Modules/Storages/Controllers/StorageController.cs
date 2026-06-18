using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lib.Modules.Storages.DTOs;
using Lib.Modules.Storages.Entities;
using Lib.Modules.Storages.Services;
using Lib.Shared.Exceptions;
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
        var userId = GetCurrentUserId();
        var result = await storageService.GetAllByUserIdAsync(userId);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StorageDto>> GetById(Guid id)
    {
        var storage = await GetStorage(id);
        var userId = GetCurrentUserId();
        if (storage.CanRead(userId)) return Ok(StorageDto.FromStorage(storage, userId));
        return NotFound(new
        {
            message = "Storage not found"
        });
    }

    [HttpPost]
    public async Task<ActionResult<StorageDto>> Create([FromBody] StorageCreateDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await storageService.CreateAsync(userId, dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<StorageDto>> Update(Guid id, [FromBody] StorageUpdateDto dto)
    {
        var storage = await GetStorage(id);
        var userId = GetCurrentUserId();

        if (!storage.CanRead(userId)) return NotFound(new { message = "Storage not found" });
        if (!storage.IsOwnedBy(userId)) return StatusCode(403, new { message = "You cannot edit this storage" });

        var updated = await storageService.UpdateAsync(id, dto, userId);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var storage = await GetStorage(id);
        var userId = GetCurrentUserId();

        if (!storage.CanRead(userId)) return NotFound(new { message = "Storage not found" });
        if (!storage.IsOwnedBy(userId)) return StatusCode(403, new { message = "You cannot delete this storage" });

        await storageService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id:guid}/config")]
    public async Task<ActionResult<StorageConfigDto>> GetConfig(Guid id)
    {
        var storage = await GetStorage(id);
        var userId = GetCurrentUserId();

        if (!storage.CanRead(userId)) return NotFound(new { message = "Storage not found" });
        if (!storage.IsOwnedBy(userId)) return StatusCode(403, new { message = "You cannot configure this storage" });

        var result = await storageService.GetConfigAsync(id);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/config")]
    public async Task<ActionResult> UpdateConfig(Guid id, [FromBody] StorageUpdateConfigDto dto)
    {
        var storage = await GetStorage(id);
        var userId = GetCurrentUserId();

        if (!storage.CanRead(userId)) return NotFound(new { message = "Storage not found" });
        if (!storage.IsOwnedBy(userId)) return StatusCode(403, new { message = "You cannot configure this storage" });

        await storageService.UpdateConfigAsync(id, dto);
        return NoContent();
    }

    [Authorize]
    [HttpGet("{id:guid}/members")]
    public async Task<ActionResult<List<StorageMemberDto>>> GetMembers(
        Guid id,
        [FromQuery] List<StorageAccessLevel>? accessLevels = null
    )
    {
        var storage = await storageService.GetRawByIdAsync(id);
        if (!storage.CanRead(GetCurrentUserId()))
        {
            return NotFound(new { message = "Project not found" });
        }

        var members = storage.AccessEntries
            .Select(a => new StorageMemberDto(
                a.UserId,
                a.IsOwner ? StorageAccessLevel.Owner : StorageAccessLevel.Write
            ));

        if (accessLevels is { Count: > 0 })
        {
            members = members.Where(m => accessLevels.Contains(m.AccessLevel));
        }

        return Ok(members.ToList());
    }

    [HttpPut("{id:guid}/access/{targetUserId:guid}")]
    public async Task<ActionResult> GrantAccess(Guid id, Guid targetUserId, [FromBody] StorageGrantAccessDto dto)
    {
        var storage = await GetStorage(id);
        var userId = GetCurrentUserId();

        if (!storage.CanRead(userId))
        {
            return NotFound(new { message = "Storage not found" });
        }
        if (!storage.IsOwnedBy(userId))
        {
            return StatusCode(403, new { message = "You cannot manage access for this project" });
        }

        await storageService.GrantAccessAsync(id, dto with { UserId = targetUserId });
        return NoContent();
    }

    [HttpDelete("{id:guid}/access/{targetUserId:guid}")]
    public async Task<ActionResult> RevokeAccess(Guid id, Guid targetUserId)
    {
        var storage = await GetStorage(id);
        var userId = GetCurrentUserId();

        if (!storage.CanRead(userId))
        {
            return NotFound(new { message = "Storage not found" });
        }

        if (!storage.IsOwnedBy(userId))
        {
            return StatusCode(403, new { message = "You cannot manage access for this project" });
        }

        await storageService.RevokeAccessAsync(id, targetUserId);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (claim is null) throw new UnauthorizedException("Unable to identify the user");
        return Guid.Parse(claim.Value);
    }

    private Task<Storage> GetStorage(Guid storageId)
    {
        return storageService.GetRawByIdAsync(storageId);
    }
}