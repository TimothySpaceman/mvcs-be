using Core.Commits;

namespace Lib.Modules.Vcs.Services;

public interface IPushService
{
    public Task UpdateCommitsChainAsync(
        Guid projectId,
        string refName,
        IEnumerable<Commit> commits,
        CancellationToken cancellationToken = default
    );
}