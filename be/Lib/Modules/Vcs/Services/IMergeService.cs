using Core.Identities;
using Core.Storage;

namespace Lib.Modules.Vcs.Services;

public enum MergeResult
{
    Success,
    RefNotFound,
    RefValueNull,
    RefMismatch,
}

public interface IMergeService
{
    public Task<MergeResult> MergeAsync(
        string title,
        Guid projectId,
        string targetRefName,
        string sourceRefName,
        HashId expectedTargetHead,
        HashId expectedSourceHead,
        UserIdentity author,
        CancellationToken cancellationToken = default
    );
}