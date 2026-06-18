using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lib.Modules.Projects.DTOs;
using Lib.Modules.Projects.Repositories;
using Lib.Modules.Projects.Services;
using Lib.Modules.Storages.Services;
using Lib.Shared.DTOs;
using Lib.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Projects.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectController(IProjectService projectService, IStorageService storageService) : ControllerBase
{
    private const int MaxItemsPerPage = 100;
    private const int MinItemsPerPage = 1;
    private const int DefaultItemsPerPage = 20;
    private const int MinPage = 1;
    
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<ProjectDto>>> Search(
        [FromQuery] int page = MinPage,
        [FromQuery] int itemsPerPage = DefaultItemsPerPage,
        [FromQuery] bool? isPublic = null,
        [FromQuery] bool? explicitAccessOnly = null,
        [FromQuery] string? search = null,
        [FromQuery] Guid? authorId = null,
        [FromQuery] Guid? storageId = null
    )
    {
        if (page < MinPage || itemsPerPage < MinItemsPerPage || itemsPerPage > MaxItemsPerPage)
        {
            return BadRequest(new { message = "Invalid pagination parameters" });
        }

        var userId = GetCurrentUserId(allowAnonymous: true);
        var filter = new ProjectFilter
        {
            Page = page,
            ItemsPerPage = itemsPerPage,
            IsPublic = isPublic,
            Search = search,
            AuthorId = authorId,
            StorageId = storageId,
            ExplicitAccessOnly = explicitAccessOnly
        };

        return Ok(await projectService.SearchAsync(filter, userId));
    }
    
    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<List<ProjectDto>>> GetMine()
    {
        var userId = GetCurrentUserId();
        return Ok(await projectService.GetAllByAuthorIdAsync(userId));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id)
    {
        var project = await projectService.GetRawByIdAsync(id);
        var userId = GetCurrentUserId(true);
        if (project.CanRead(userId)) return Ok(ProjectDto.FromProject(project, userId));
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
        if (storageAccess is null)
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
        var userId = GetCurrentUserId();
        if (project is null || project.AuthorId != userId)
        {
            return NotFound(new
            {
                message = "Project not found"
            });
        }

        var updated = await projectService.UpdateAsync(id, dto, userId);
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
    
    [Authorize]
    [HttpGet("{projectId:guid}/members")]
    public async Task<ActionResult<List<ProjectMemberDto>>> GetMembers(
        Guid projectId,
        [FromQuery] List<ProjectAccessLevel>? accessLevels = null
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        if (!project.CanReadExplicitly(GetCurrentUserId()))
        {
            return NotFound(new { message = "Project not found" });
        }

        var members = project.AccessEntries
            .Select(a => new ProjectMemberDto(a.UserId, a.CanWrite ? ProjectAccessLevel.Write : ProjectAccessLevel.Read))
            .Append(new ProjectMemberDto(project.AuthorId, ProjectAccessLevel.Owner));

        if (accessLevels is { Count: > 0 })
        {
            members = members.Where(m => accessLevels.Contains(m.AccessLevel));
        }

        return Ok(members.ToList());
    }
    
    [Authorize]
    [HttpPut("{projectId:guid}/access/{targetUserId:guid}")]
    public async Task<ActionResult> GrantAccess(Guid projectId, Guid targetUserId, [FromBody] ProjectGrantAccessDto dto)
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId();

        if (!project.CanReadExplicitly(userId)) 
        {
            return NotFound(new { message = "Project not found" });
        }
        if (project.AuthorId != userId) 
        {
            return StatusCode(403, new { message = "You cannot manage access for this project" });
        }

        await projectService.GrantAccessAsync(projectId, targetUserId, dto.AccessType);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{projectId:guid}/access/{targetUserId:guid}")]
    public async Task<ActionResult> RevokeAccess(Guid projectId, Guid targetUserId)
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId();

        if (!project.CanReadExplicitly(userId)) 
        {
            return NotFound(new { message = "Project not found" });
        }
        if (project.AuthorId != userId) 
        {
            return StatusCode(403, new { message = "You cannot manage access for this project" });
        }

        await projectService.RevokeAccessAsync(projectId, targetUserId);
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