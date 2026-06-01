using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Lib.Modules.Auth.DTOs;
using Lib.Modules.Auth.Services;
using Lib.Modules.Users.DTOs;
using Lib.Modules.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Lib.Modules.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    IAuthService authService,
    IUserService userService,
    IConfiguration config,
    IHostEnvironment env
) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {
        var user = await authService.RegisterAsync(registerDto);
        return Created($"/api/users/{user.Id}", user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginWithCredentialsDto dto,
        [FromHeader(Name = "X-Client-Type")] string? clientType = null
    )
    {
        var tokens = await authService.LoginWithCredentialsAsync(
            dto,
            DeviceWithIpDto.FromHttpContext(HttpContext)
        );

        return ProcessTokenPair(tokens, clientType?.ToLowerInvariant() == "desktop");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutDto? dto = null)
    {
        var refreshToken = dto?.RefreshToken ?? Request.Cookies[config["JwtSettings:Refresh:CookieName"]!];
        if (refreshToken is not null) await authService.LogoutAsync(refreshToken);

        Response.Cookies.Delete(config["JwtSettings:Access:CookieName"]!);
        Response.Cookies.Delete(config["JwtSettings:Refresh:CookieName"]!);
        Response.Cookies.Delete(config["Auth:Metadata:CookieName"]!);

        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshDto? dto = null,
        [FromHeader(Name = "X-Client-Type")] string? clientType = null
    )
    {
        var refreshToken = dto?.RefreshToken ?? Request.Cookies[config["JwtSettings:Refresh:CookieName"]!];
        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest(new
            {
                message = "Refresh token is not provided"
            });

        var newTokens = await authService.RefreshAsync(refreshToken);
        return ProcessTokenPair(newTokens, clientType?.ToLowerInvariant() == "desktop");
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (userIdClaim is null)
        {
            return Unauthorized(new
            {
                message = "Unable to identify the user"
            });
        }

        var userId = Guid.Parse(userIdClaim.Value);

        var user = await userService.GetByIdAsync(userId);
        if (user is null)
        {
            return NotFound(new
            {
                message = "User not found"
            });
        }

        return Ok(user);
    }

    private IActionResult ProcessTokenPair(TokenPairDto tokens, bool isDesktop)
    {
        if (isDesktop) return Ok(tokens);
        SetAuthCookies(tokens);
        return Ok(new { message = "Success" });
    }

    private void SetAuthCookies(TokenPairDto tokens)
    {
        var isDev = env.IsDevelopment();
        var sameSite = isDev ? SameSiteMode.None : SameSiteMode.Strict;

        var accessCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = sameSite,
            Expires = DateTimeOffset.UtcNow.AddMinutes(config.GetValue<double>("JwtSettings:Access:ExpiryMinutes"))
        };

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = sameSite,
            Expires = DateTimeOffset.UtcNow.AddMinutes(config.GetValue<double>("JwtSettings:Refresh:ExpiryMinutes"))
        };

        var metadataCookieOptions = new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = sameSite,
            Expires = DateTimeOffset.UtcNow.AddMinutes(config.GetValue<double>("Auth:Metadata:ExpiryMinutes"))
        };

        var metadataString = JsonSerializer.Serialize(
            UserMetadataDto.FromUserDto(tokens.User),
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
        );

        Response.Cookies.Append(config["JwtSettings:Access:CookieName"]!, tokens.AccessToken, accessCookieOptions);
        Response.Cookies.Append(config["JwtSettings:Refresh:CookieName"]!, tokens.RefreshToken, refreshCookieOptions);
        Response.Cookies.Append(config["Auth:Metadata:CookieName"]!, metadataString, metadataCookieOptions);
    }
}