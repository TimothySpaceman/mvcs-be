using Core.Commits;
using Lib.Modules.Projects.Entities;

namespace Lib.Modules.Vcs.Services;

public interface IPushService
{
    public Task UpdateCommitsChainAsync(
        Project project,
        string refName,
        IEnumerable<Commit> commits,
        CancellationToken cancellationToken = default
    );

    public Task UpdateCommitsChainAsync(
        Guid projectId,
        string refName,
        IEnumerable<Commit> commits,
        CancellationToken cancellationToken = default
    );
}