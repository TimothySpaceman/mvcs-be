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
        var userId = GetUserId();
        return await projectService.GetAllByAuthorIdAsync(userId);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id)
    {
        var project = await projectService.GetRawByIdAsync(id);
        var userId = GetUserId(true);
        var isVisible = (
            project.IsPublic ||
            project.AuthorId == userId ||
            project.AccessEntries.Any(a => a.UserId == userId)
        );
        return isVisible ? Ok(ProjectDto.FromProject(project)) : NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] ProjectCreateDto createDto)
    {
        var userId = GetUserId();

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
        if (project is null || project.AuthorId != GetUserId()) return NotFound();

        var updated = await projectService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> Delete(Guid id)
    {
        var project = await projectService.GetByIdAsync(id);
        if (project is null || project.AuthorId != GetUserId()) return NotFound();
        await projectService.DeleteAsync(id);
        return NoContent();
    }

    private Guid GetUserId()
    {
        return (Guid)GetUserId(false)!;
    }

    private Guid? GetUserId(bool allowAnonymous)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (claim is null && !allowAnonymous) throw new UnauthorizedException("Unable to identify the user");
        return claim is null ? null : Guid.Parse(claim.Value);
    }
}