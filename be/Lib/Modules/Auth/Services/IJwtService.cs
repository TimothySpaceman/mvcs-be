using System.Security.Claims;
using Lib.Modules.Users.DTOs;

namespace Lib.Modules.Auth.Services;

public interface IJwtService
{
    public string GenerateAccessToken(UserDto user, Guid sessionId);
    public string GenerateRefreshToken(UserDto user);

    public ClaimsPrincipal? Validate(string token, string settingsPrefix); 
}