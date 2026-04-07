using Lib.Modules.Auth.Entities;

namespace Lib.Modules.Auth.Repositories;

public interface IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByIdAsync(Guid id);
    public Task<RefreshToken?> GetBySessionIdAsync(Guid sessionId);
    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
    public Task<bool> ExistsBySessionIdAsync(Guid sessionId);
    public Task AddAsync(RefreshToken refreshToken);
    public void Delete(RefreshToken refreshToken);
    public Task SaveChangesAsync();
}