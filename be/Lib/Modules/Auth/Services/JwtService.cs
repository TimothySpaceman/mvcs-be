using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lib.Modules.Users.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Lib.Modules.Auth.Services;

public class JwtService(IConfiguration config) : IJwtService
{
    public string GenerateAccessToken(UserDto user, Guid sessionId)
    {
        var secret = config["JwtSettings:Access:Secret"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sid, sessionId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: config["JwtSettings:Access:Issuer"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(config.GetValue<double>("JwtSettings:Access:ExpiryMinutes")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(UserDto user)
    {
        var secret = config["JwtSettings:Refresh:Secret"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: config["JwtSettings:Refresh:Issuer"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(config.GetValue<double>("JwtSettings:Refresh:ExpiryMinutes")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public ClaimsPrincipal? Validate(string token, string settingsPrefix)
    {
        var secret = config[$"JwtSettings:{settingsPrefix}:Secret"]!;
        var issuer = config[$"JwtSettings:{settingsPrefix}:Issuer"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            return new JwtSecurityTokenHandler()
                .ValidateToken(token, validationParams, out _);
        }
        catch
        {
            return null;
        }
    }
}