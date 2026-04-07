using Lib.Modules.Auth.DTOs;

namespace Lib.Modules.Auth.Services;

public interface ISessionService
{
    public Task<IEnumerable<SessionDto>> GetAllByUserIdAsync(Guid userId);
    public Task<TokenPairDto> CreateAsync(SessionCreateDto sessionCreateDto);
    public Task<TokenPairDto> RefreshAsync(string refreshToken);
    public Task<bool> RevokeByTokenAsync(string refreshToken);
    public Task RevokeAllByUserIdAsync(Guid userId);
}