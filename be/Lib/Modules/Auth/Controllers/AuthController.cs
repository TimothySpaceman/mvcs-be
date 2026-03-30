using Lib.Modules.Auth.DTOs;
using Lib.Modules.Auth.Services;
using Lib.Modules.Users.DTOs;
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

        var clientType = Request.Headers["X-Client-Type"].FirstOrDefault()?.ToLowerInvariant();
        if (clientType == "desktop") return Ok(tokens);

        SetAuthCookies(tokens);
        return Ok(new { message = "Login successful" });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(config["JwtSettings:Access:CookieName"]!);
        Response.Cookies.Delete(config["JwtSettings:Refresh:CookieName"]!);

        return Ok(new { message = "Logged out successfully" });
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