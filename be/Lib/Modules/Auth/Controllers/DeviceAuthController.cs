using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lib.Modules.Auth.DTOs;
using Lib.Modules.Auth.Entities;
using Lib.Modules.Auth.Services;
using Lib.Modules.Users.DTOs;
using Lib.Modules.Users.Services;
using Lib.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Lib.Modules.Auth.Controllers;

[ApiController]
[Route("api/auth/device")]
public class DeviceAuthController(
    IDeviceAuthService deviceAuthService,
    ISessionService sessionService,
    IUserService userService,
    IConfiguration config
) : ControllerBase
{
    [HttpPost("start")]
    public async Task<ActionResult<DeviceAuthStartDto>> Start()
    {
        var deviceDto = DeviceWithIpDto.FromHttpContext(HttpContext);
        var transaction = await deviceAuthService.StartTransactionAsync(deviceDto);

        var expirationMinutes = config.GetValue<double>("Auth:DeviceFlow:TransactionExpiryMinutes");
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes);
        var result = new DeviceAuthStartDto(
            transaction.UserCode,
            transaction.DeviceCode,
            config.GetValue<string>("Auth:DeviceFlow:VerificationUrl")!,
            expiresAt,
            config.GetValue<double>("Auth:DeviceFlow:PollingIntervalSeconds")
        );
        return Ok(result);
    }

    [Authorize]
    [HttpGet("info")]
    public async Task<ActionResult<DeviceInfo>> GetInfo([FromQuery] string userCode)
    {
        if (string.IsNullOrEmpty(userCode))
            return BadRequest(new
            {
                message = "User code is not provided"
            });

        var transaction = await deviceAuthService.GetByUserCodeAsync(userCode);

        if (transaction is not null) return Ok(transaction.DeviceInfo);
        return NotFound(new
        {
            message = "No pending transaction found by this user code"
        });
    }

    [Authorize]
    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm([FromQuery] string userCode)
    {
        if (string.IsNullOrEmpty(userCode))
            return BadRequest(new
            {
                message = "User code is not provided"
            });

        var user = await GetCurrentUser();

        await deviceAuthService.ConfirmByUserCodeAsync(userCode, user.Id);
        return Ok(new
        {
            message = "Transaction confirmed successfully"
        });
    }

    [HttpPost("poll")]
    public async Task<IActionResult> Poll([FromBody] DeviceAuthPollDto pollDto)
    {
        var transaction = await deviceAuthService.GetByDeviceCodeAsync(pollDto.DeviceCode);
        if (transaction is null)
        {
            return NotFound(new
            {
                message = "invalid_code"
            });
        }

        if (transaction.IsAborted)
        {
            return BadRequest(new
            {
                message = "aborted"
            });
        }

        if (!transaction.IsConfirmed)
        {
            return BadRequest(new
            {
                message = "pending"
            });
        }

        var tokens = await sessionService.CreateAsync(new SessionCreateDto(
            (Guid)transaction.UserId!,
            transaction.DeviceInfo,
            transaction.IpAddress
        ));

        await deviceAuthService.CloseByDeviceCodeAsync(transaction.DeviceCode);

        return Ok(tokens);
    }

    private async Task<UserDto> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (userIdClaim is null) throw new UnauthorizedException("Unable to identify the user");

        var userId = Guid.Parse(userIdClaim.Value);

        var user = await userService.GetByIdAsync(userId);
        if (user is null) throw new NotFoundException("User not found");

        return user;
    }
}