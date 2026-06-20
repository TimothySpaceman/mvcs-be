using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lib.Modules.Projects.Services;
using Lib.Modules.Tasks.DTOs;
using Lib.Modules.Tasks.Repositories;
using Lib.Modules.Tasks.Services;
using Lib.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Tasks.Controllers;

[Authorize]
[ApiController]
[Route("api/projects")]
public class TasksController(
    IProjectService projectService,
    ITaskService taskService
) : ControllerBase
{
    [HttpGet("{projectId:guid}/tasks")]
    public async Task<ActionResult<List<TaskDto>>> GetAll(
        [FromRoute] Guid projectId,
        [FromQuery] Guid? assignedUserId,
        [FromQuery] Entities.TaskStatus? status
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        if (!project.CanReadExplicitly(GetCurrentUserId()))
        {
            return NotFound(new { message = "Project not found" });
        }

        var tasks = await taskService.GetAllAsync(new TaskFilter
        {
            ProjectId = projectId,
            AssignedUserId = assignedUserId,
            Status = status
        });

        return Ok(tasks);
    }

    [HttpGet("{projectId:guid}/tasks/{taskId:guid}")]
    public async Task<ActionResult<TaskDto>> GetById(
        [FromRoute] Guid projectId,
        [FromRoute] Guid taskId
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        if (!project.CanReadExplicitly(GetCurrentUserId()))
        {
            return NotFound(new { message = "Project not found" });
        }

        try
        {
            return Ok(await taskService.GetByIdAsync(taskId));
        }
        catch (NotFoundException)
        {
            return NotFound(new { message = "Task not found" });
        }
    }

    [HttpPost("{projectId:guid}/tasks")]
    public async Task<ActionResult<TaskDto>> Create(
        [FromRoute] Guid projectId,
        [FromBody] CreateTaskDto dto
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId()!.Value;
        if (!project.CanReadExplicitly(userId))
        {
            return NotFound(new { message = "Project not found" });
        }
        if (!project.CanWrite(userId))
        {
            return StatusCode(403, new { message = "You cannot manage tasks for this project" });
        }

        var task = await taskService.CreateAsync(projectId, userId, dto);
        return Ok(task);
    }
    
    [HttpPatch("{projectId:guid}/tasks/{taskId:guid}")]
    public async Task<ActionResult<TaskDto>> Update(
        [FromRoute] Guid projectId,
        [FromRoute] Guid taskId,
        [FromBody] UpdateTaskDto dto
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId()!.Value;
        if (!project.CanReadExplicitly(userId))
        {
            return NotFound(new { message = "Project not found" });
        }
        if (!project.CanWrite(userId))
        {
            return StatusCode(403, new { message = "You cannot manage tasks for this project" });
        }

        try
        {
            return Ok(await taskService.UpdateAsync(taskId, dto));
        }
        catch (NotFoundException)
        {
            return NotFound(new { message = "Task not found" });
        }
    }

    [HttpPost("{projectId:guid}/tasks/{taskId:guid}/assignments")]
    public async Task<ActionResult> Assign(
        [FromRoute] Guid projectId,
        [FromRoute] Guid taskId,
        [FromBody] TaskAssignmentDto dto
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId()!.Value;
        if (!project.CanReadExplicitly(userId))
        {
            return NotFound(new { message = "Project not found" });
        }
        if (!project.CanWrite(userId))
        {
            return StatusCode(403, new { message = "You cannot manage tasks for this project" });
        }

        try
        {
            await taskService.AssignUserAsync(taskId, dto.UserId);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound(new { message = "Task not found" });
        }
    }

    [HttpDelete("{projectId:guid}/tasks/{taskId:guid}/assignments/{userId:guid}")]
    public async Task<ActionResult> Unassign(
        [FromRoute] Guid projectId,
        [FromRoute] Guid taskId,
        [FromRoute] Guid userId
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        var currentUserId = GetCurrentUserId()!.Value;
        if (!project.CanReadExplicitly(currentUserId))
        {
            return NotFound(new { message = "Project not found" });
        }
        if (!project.CanWrite(currentUserId))
        {
            return StatusCode(403, new { message = "You cannot manage tasks for this project" });
        }

        try
        {
            await taskService.UnassignUserAsync(taskId, userId);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound(new { message = "Task not found" });
        }
    }

    [HttpDelete("{projectId:guid}/tasks/{taskId:guid}")]
    public async Task<ActionResult> Delete(
        [FromRoute] Guid projectId,
        [FromRoute] Guid taskId
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId()!.Value;
        if (!project.CanReadExplicitly(userId))
        {
            return NotFound(new { message = "Project not found" });
        }
        if (!project.CanWrite(userId))
        {
            return StatusCode(403, new { message = "You cannot manage tasks for this project" });
        }

        try
        {
            await taskService.DeleteAsync(taskId);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound(new { message = "Task not found" });
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        return userIdClaim is null ? null : Guid.Parse(userIdClaim.Value);
    }
}