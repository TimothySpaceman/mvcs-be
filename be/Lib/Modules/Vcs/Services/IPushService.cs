using Core.Commits;
using Core.Storage;
using Lib.Modules.Projects.Entities;

namespace Lib.Modules.Vcs.Services;

public enum PushResult
{
    Success,
    RefMismatch,
}

public interface IPushService
{
    public Task<PushResult> ApplyPushAsync(
        Guid projectId,
        string refName,
        HashId? expectedHead,
        IEnumerable<Commit> commits,
        CancellationToken cancellationToken = default
    );

    public Task<PushResult> ApplyPushAsync(
        Project project,
        string refName,
        HashId? expectedHead,
        IEnumerable<Commit> commits,
        CancellationToken cancellationToken = default
    );
}