using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lib.Modules.Projects.DTOs;
using Lib.Modules.Projects.Services;
using Lib.Modules.Storages.Services;
using Lib.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Projects.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectController(IProjectService projectService, IStorageService storageService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetMine()
    {
        var userId = GetCurrentUserId();
        return await projectService.GetAllByAuthorIdAsync(userId);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id)
    {
        var project = await projectService.GetRawByIdAsync(id);
        var userId = GetCurrentUserId(true);
        if (project.CanRead(userId)) return Ok(ProjectDto.FromProject(project));
        return NotFound(new { message = "Project not found" });
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] ProjectCreateDto createDto)
    {
        var userId = GetCurrentUserId();

        var storage = await storageService.GetRawByIdAsync(createDto.StorageId);
        if (storage.IsPublic) return await projectService.CreateAsync(userId, createDto);

        var storageAccess = storage.AccessEntries.FirstOrDefault(a => a.UserId == userId);
        if (storageAccess is null || !storageAccess.CanRead)
        {
            return StatusCode(404, new { message = "Storage not found" });
        }

        if (!storageAccess.CanWrite)
        {
            return StatusCode(403, new { message = "You cannot create a project on this storage" });
        }

        return await projectService.CreateAsync(userId, createDto);
    }

    [Authorize]
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> Update(Guid id, [FromBody] ProjectUpdateDto dto)
    {
        var project = await projectService.GetByIdAsync(id);
        if (project is null || project.AuthorId != GetCurrentUserId())
        {
            return NotFound(new
            {
                message = "Project not found"
            });
        }

        var updated = await projectService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> Delete(Guid id)
    {
        var project = await projectService.GetByIdAsync(id);
        if (project is null || project.AuthorId != GetCurrentUserId())
        {
            return NotFound(new { message = "Project not found" });
        }

        await projectService.DeleteAsync(id);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        return (Guid)GetCurrentUserId(false)!;
    }

    private Guid? GetCurrentUserId(bool allowAnonymous)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (claim is null && !allowAnonymous) throw new UnauthorizedException("Unable to identify the user");
        return claim is null ? null : Guid.Parse(claim.Value);
    }
}