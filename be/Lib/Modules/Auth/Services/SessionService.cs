using System.Security.Cryptography;
using System.Text;
using Lib.Modules.Auth.DTOs;
using Lib.Modules.Auth.Entities;
using Lib.Modules.Auth.Repositories;
using Lib.Modules.Users.DTOs;
using Lib.Modules.Users.Services;
using Lib.Shared.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Lib.Modules.Auth.Services;

public class SessionService(
    ISessionRepository sessionRepository,
    IRefreshTokenRepository tokenRepository,
    IUserService userService,
    IJwtService jwtService,
    IConfiguration config
) : ISessionService
{
    public async Task<IEnumerable<SessionDto>> GetAllByUserIdAsync(Guid userId)
    {
        var sessions = await sessionRepository.GetByUserIdAsync(userId);
        return sessions.Select(SessionDto.FromSession);
    }

    public async Task<TokenPairDto> CreateAsync(SessionCreateDto createDto)
    {
        var user = await userService.GetByIdAsync(createDto.UserId);
        if (user is null) throw new InvalidOperationException("User not found");

        var session = Session.Create(
            createDto.UserId,
            createDto.DeviceInfo,
            createDto.IpAddress
        );
        await sessionRepository.AddAsync(session);
        await sessionRepository.SaveChangesAsync();

        var tokenPairDto = CreateTokens(user);
        await AttachRefreshToken(tokenPairDto.RefreshToken, session.Id);
        await tokenRepository.SaveChangesAsync();

        return tokenPairDto;
    }

    public async Task<TokenPairDto> RefreshAsync(string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);
        var session = await sessionRepository.GetByTokenHashAsync(tokenHash);
        if (session is null || session.IsRevoked)
        {
            throw new UnauthorizedException("Attached session is revoked or does not exist");
        }

        if (session.RefreshToken.IsExpired)
        {
            throw new UnauthorizedException("Refresh token expired");
        }

        var oldToken = session.RefreshToken;
        var tokenPairDto = CreateTokens(UserDto.FromUser(session.User));
        await AttachRefreshToken(tokenPairDto.RefreshToken, session.Id);
        tokenRepository.Delete(oldToken);
        await tokenRepository.SaveChangesAsync();

        session.Refresh();
        await sessionRepository.SaveChangesAsync();

        return tokenPairDto;
    }

    public async Task<bool> RevokeByTokenAsync(string refreshToken)
    {
        var session = await sessionRepository.GetByTokenHashAsync(HashToken(refreshToken));
        if (session is null) return false;

        session.Revoke();
        await sessionRepository.SaveChangesAsync();
        return true;
    }

    public async Task RevokeAllByUserIdAsync(Guid userId)
    {
        var sessions = await sessionRepository.GetByUserIdAsync(userId);
        foreach (var session in sessions) session.Revoke();
        await sessionRepository.SaveChangesAsync();
    }

    private TokenPairDto CreateTokens(UserDto user)
    {
        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshToken(user);
        return new TokenPairDto(accessToken, refreshToken);
    }

    private string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private async Task AttachRefreshToken(string refreshToken, Guid sessionId)
    {
        var refreshExpiration = DateTimeOffset.UtcNow.AddMinutes(
            config.GetValue<double>("JwtSettings:Refresh:ExpiryMinutes")
        );
        var refreshRecord = RefreshToken.Create(
            sessionId,
            HashToken(refreshToken),
            refreshExpiration
        );
        await tokenRepository.AddAsync(refreshRecord);
    }
}