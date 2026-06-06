using Core.Storage;
using Lib.Modules.Vcs.Entities;
using Lib.Modules.Vcs.Repository;

namespace Lib.Modules.Vcs.Services;

public class RefService(IRefRepository refRepository) : IRefService
{
    public Task<List<RefEntity>> GetAllRefsAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        return refRepository.GetAllAsync(projectId, cancellationToken);
    }
    
    public async Task<HashId?> GetRefValueAsync(
        Guid projectId,
        string refName,
        CancellationToken cancellationToken = default
    )
    {
        var entry = await refRepository.GetAsync(projectId, refName, cancellationToken);
        return entry?.CommitId;
    }

    public async Task SetRefValueAsync(
        Guid projectId,
        string refName,
        HashId value,
        CancellationToken cancellationToken = default
    )
    {
        var entry = await refRepository.GetAsync(projectId, refName, cancellationToken);
        if (entry is not null)
        {
            entry.SetCommitId(value);
        }
        else
        {
            await refRepository.AddAsync(RefEntity.Create(projectId, refName, value), cancellationToken);
        }

        await refRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteRefAsync(
        Guid projectId,
        string refName,
        CancellationToken cancellationToken = default
    )
    {
        var entry = await refRepository.GetAsync(projectId, refName, cancellationToken);
        if (entry is null) return false;
        refRepository.Delete(entry);
        await refRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}