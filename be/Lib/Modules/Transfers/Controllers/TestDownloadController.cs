using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lib.Modules.Transfers.Adapters;
using Lib.Modules.Transfers.Services;
using Lib.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Transfers.Controllers;

[ApiController]
[Route("api")]
public class TestDownloadController(ITransferService transferService) : ControllerBase
{
    [Authorize]
    [HttpGet("storages/{storageId}/contents")]
    public async Task DownloadFileAsync(
        [FromQuery] string filePath,
        [FromRoute] string storageId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var storageGuid = Guid.Parse(storageId);

        ByteRange? range = null;
        if (Request.Headers.TryGetValue("Range", out var rangeHeader))
        {
            range = ByteRange.FromHeaderValue(rangeHeader!);
        }

        var totalLength = await transferService.GetContentLengthAsync(
            storageGuid,
            userId,
            filePath,
            cancellationToken
        );

        var rangeStart = range?.Start ?? 0;
        var rangeEnd = range?.End ?? totalLength - 1;
        var responseLength = rangeEnd - rangeStart + 1;

        await using var stream = await transferService.GetContentAsync(
            storageGuid,
            userId,
            filePath,
            range,
            cancellationToken
        );

        var bufferingFeature = HttpContext.Features.Get<IHttpResponseBodyFeature>();
        bufferingFeature?.DisableBuffering();

        Response.StatusCode = range is null ? 200 : 206;
        Response.Headers.Append("Accept-Ranges", "bytes");
        Response.Headers.Append("Content-Type", "application/octet-stream");
        Response.Headers.Append("Content-Length", responseLength.ToString());
        if (range is not null)
        {
            Response.Headers.Append("Content-Range", $"bytes {rangeStart}-{rangeEnd}/{totalLength}");
        }

        await stream.CopyToAsync(Response.Body, cancellationToken);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (userIdClaim is null) throw new UnauthorizedException("Unable to identify the user");

        return Guid.Parse(userIdClaim.Value);
    }
}