using Lib.Modules.Vcs.Entities;

namespace Lib.Modules.Vcs.Repository;

public interface IMergeRequestRepository
{
    public Task<MergeRequest?> GetAsync(Guid projectId, Guid id, CancellationToken cancellationToken = default);
    public Task<List<MergeRequest>> GetAllByProjectIdAsync(
        Guid projectId,
        int page,
        int itemsPerPage,
        CancellationToken cancellationToken = default
    );
    public Task<int> CountByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    public Task AddAsync(MergeRequest mergeRequest, CancellationToken cancellationToken = default);
    public Task SaveChangesAsync(CancellationToken cancellationToken = default);
}