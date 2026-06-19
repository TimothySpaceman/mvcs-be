using Lib.Modules.Auth.DTOs;
using Lib.Shared.DTOs;

namespace Lib.Modules.Auth.Services;

public interface ISessionService
{
    public Task<CursorPagedResultDto<SessionDto, Guid?>> GetPageByUserIdAsync(
        Guid userId,
        Guid? cursor,
        int limit
    );
    public Task<TokenPairDto> CreateAsync(SessionCreateDto sessionCreateDto);
    public Task<TokenPairDto> RefreshAsync(string refreshToken);
    public Task<bool> RevokeByIdAsync(Guid sessionId, Guid requestingUserId);
    public Task<bool> RevokeByTokenAsync(string refreshToken);
    public Task RevokeAllByUserIdAsync(Guid userId);
}