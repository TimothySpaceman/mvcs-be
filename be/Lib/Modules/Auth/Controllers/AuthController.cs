using Lib.Modules.Auth.DTOs;
using Lib.Modules.Auth.Services;
using Lib.Modules.Users.DTOs;
using Lib.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Lib.Modules.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    IAuthService authService,
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
    public async Task<IActionResult> Login([FromBody] LoginWithCredentialsDto dto)
    {
        var tokens = await authService.LoginWithCredentialsAsync(
            dto,
            DeviceWithIpDto.FromHttpContext(HttpContext)
        );

        return ProcessTokenPair(tokens);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
    {
        var token = RequireRefreshToken(dto.RefreshToken);
        await authService.LogoutAsync(token);

        Response.Cookies.Delete(config["JwtSettings:Access:CookieName"]!);
        Response.Cookies.Delete(config["JwtSettings:Refresh:CookieName"]!);

        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto dto)
    {
        var token = RequireRefreshToken(dto.RefreshToken);

        var tokens = await authService.RefreshAsync(token);
        return ProcessTokenPair(tokens);
    }

    private string RequireRefreshToken(string? plainSource)
    {
        var token = plainSource ?? Request.Cookies[config["JwtSettings:Access:CookieName"]!];
        if (string.IsNullOrEmpty(token)) throw new BadRequestException("No refresh token provided");
        return token;
    }

    private IActionResult ProcessTokenPair(TokenPairDto tokens)
    {
        var clientType = Request.Headers["X-Client-Type"].FirstOrDefault()?.ToLowerInvariant();
        if (clientType == "desktop") return Ok(tokens);

        SetAuthCookies(tokens);
        return Ok(new { message = "Login successful" });
    }

    private void SetAuthCookies(TokenPairDto tokens)
    {
        var accessCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !env.IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(config.GetValue<double>("JwtSettings:Access:ExpiryMinutes"))
        };

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !env.IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(config.GetValue<double>("JwtSettings:Refresh:ExpiryMinutes"))
        };

        Response.Cookies.Append(config["JwtSettings:Access:CookieName"]!, tokens.AccessToken, accessCookieOptions);
        Response.Cookies.Append(config["JwtSettings:Refresh:CookieName"]!, tokens.RefreshToken, refreshCookieOptions);
    }
}