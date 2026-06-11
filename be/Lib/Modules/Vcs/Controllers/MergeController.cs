using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Identities;
using Lib.Modules.Projects.Services;
using Lib.Modules.Users.Services;
using Lib.Modules.Vcs.DTOs;
using Lib.Modules.Vcs.Repository;
using Lib.Modules.Vcs.Services;
using Lib.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Vcs.Controllers;

[ApiController]
[Route("api/projects")]
public class MergeController(
    IUserService userService,
    IProjectService projectService,
    IMergeService mergeService,
    IMergeRequestRepository mergeRequestRepository
) : ControllerBase
{
    private const int MinPage = 1;
    private const int DefaultItemsPerPage = 20;
    private const int MinItemsPerPage = 1;
    private const int MaxItemsPerPage = 100;

    [Authorize]
    [HttpGet("{projectId:guid}/vcs/merge-requests")]
    public async Task<ActionResult<PagedResultDto<MergeRequestDto>>> GetMergeRequests(
        [FromRoute] Guid projectId,
        [FromQuery] int page = MinPage,
        [FromQuery] int itemsPerPage = DefaultItemsPerPage,
        CancellationToken cancellationToken = default
    )
    {
        if (page < MinPage || itemsPerPage < MinItemsPerPage || itemsPerPage > MaxItemsPerPage)
        {
            return BadRequest(new { message = "Invalid pagination parameters" });
        }

        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId();
        if (!project.CanRead(userId))
        {
            return NotFound(new { message = "Project not found" });
        }

        var items = await mergeRequestRepository.GetAllByProjectIdAsync(
            projectId,
            page,
            itemsPerPage,
            cancellationToken
        );
        var total = await mergeRequestRepository.CountByProjectIdAsync(projectId, cancellationToken);

        return Ok(new PagedResultDto<MergeRequestDto>(
            items.Select(MergeRequestDto.FromDomain),
            page,
            itemsPerPage,
            total
        ));
    }

    [Authorize]
    [HttpPost("{projectId:guid}/vcs/merge-requests")]
    public async Task<ActionResult> Merge(
        [FromRoute] Guid projectId,
        [FromBody] MergeRequestBodyDto bodyDto,
        CancellationToken cancellationToken
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        var userId = GetCurrentUserId()!.Value;

        if (!project.CanRead(userId))
        {
            return NotFound(new
            {
                message = "Project not found"
            });
        }

        if (!project.CanWrite(userId))
        {
            return StatusCode(403, new { message = "You cannot perform merges in this project" });
        }

        if (bodyDto.TargetRefName == bodyDto.SourceRefName)
        {
            return BadRequest(new { message = "Cannot merge a branch into itself" });
        }

        var user = await userService.GetByIdAsync(userId);
        if (user is null)
        {
            return Unauthorized(new
            {
                message = "Unable to fetch user data"
            });
        }

        var result = await mergeService.MergeAsync(
            bodyDto.Title,
            projectId,
            bodyDto.TargetRefName,
            bodyDto.SourceRefName,
            bodyDto.ExpectedTargetHead,
            bodyDto.ExpectedSourceHead,
            new UserIdentity(
                user.Id,
                user.DisplayName,
                user.Email
            ),
            cancellationToken
        );

        return result switch
        {
            MergeResult.RefMismatch => Conflict(new { message = "Ref values mismatch detected" }),
            MergeResult.RefNotFound => NotFound(new { message = "Ref not found" }),
            MergeResult.RefValueNull => UnprocessableEntity(new { message = "Cannot perform merge on empty branches" }),
            MergeResult.Success => NoContent(),
            _ => throw new UnreachableException()
        };
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        return userIdClaim is null ? null : Guid.Parse(userIdClaim.Value);
    }
}