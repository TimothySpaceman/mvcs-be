using Core.Storage;
using Lib.Modules.Vcs.Entities;

namespace Lib.Modules.Vcs.Services;

public interface IRefService
{
    public Task<List<RefEntity>> GetAllRefsAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    );
    
    public Task<HashId?> GetRefValueAsync(
        Guid projectId,
        string refName,
        CancellationToken cancellationToken = default
    );

    public Task SetRefValueAsync(
        Guid projectId,
        string refName,
        HashId value,
        CancellationToken cancellationToken = default
    );

    public Task<bool> DeleteRefAsync(
        Guid projectId,
        string refName,
        CancellationToken cancellationToken = default
    );
}