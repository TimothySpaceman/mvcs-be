using System.Data;
using Core.Commits;
using Core.Diffing;
using Core.Identities;
using Core.Storage;
using Lib.Infrastructure.Vcs;
using Lib.Modules.Vcs.Entities;
using Lib.Modules.Vcs.Repository;
using Lib.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Vcs.Services;

public class MergeService(
    IRefRepository refRepository,
    ICommitRepository commitRepository,
    IMergeRequestRepository mergeRequestRepository,
    ICommitService commitService,
    VcsDbContext vcsDbContext
) : IMergeService
{
    private static readonly DiffService DiffService = new();

    public async Task<MergeResult> MergeAsync(
        string title,
        Guid projectId,
        string targetRefName,
        string sourceRefName,
        HashId expectedTargetHead,
        HashId expectedSourceHead,
        UserIdentity author,
        CancellationToken cancellationToken = default
    )
    {
        if (author.Id is null) throw new BadRequestException("Author id cannot be null");

        await using var tx = await vcsDbContext.Database.BeginTransactionAsync(
            IsolationLevel.RepeatableRead,
            cancellationToken
        );

        var (targetRefEntry, targetError) = await GetRefEntryAsync(
            projectId,
            targetRefName,
            expectedTargetHead,
            cancellationToken
        );
        if (targetError is not null) return targetError.Value;
        var targetTip = targetRefEntry!.CommitId!.Value;

        var (sourceRefEntry, sourceError) = await GetRefEntryAsync(
            projectId,
            sourceRefName,
            expectedSourceHead,
            cancellationToken
        );
        if (sourceError is not null) return sourceError.Value;
        var sourceTip = sourceRefEntry!.CommitId!.Value;

        var mergeCommitId = await CreateMergeCommit(
            projectId,
            title,
            targetRefName,
            sourceRefName,
            targetTip,
            sourceTip,
            author,
            cancellationToken
        );

        targetRefEntry.SetCommitId(mergeCommitId);

        await CreateMergeRequest(
            author.Id.Value,
            projectId,
            title,
            targetRefName,
            sourceRefName,
            mergeCommitId,
            cancellationToken
        );

        await mergeRequestRepository.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return MergeResult.Success;
    }

    private async Task<(RefEntity? entry, MergeResult? error)> GetRefEntryAsync(
        Guid projectId,
        string refName,
        HashId expectedHead,
        CancellationToken cancellationToken = default
    )
    {
        var refEntry = await refRepository.GetForUpdateAsync(projectId, refName, cancellationToken);
        if (refEntry is null) return (null, MergeResult.RefNotFound);
        if (refEntry.CommitId is null) return (null, MergeResult.RefValueNull);
        if (refEntry.CommitId != expectedHead) return (null, MergeResult.RefMismatch);
        return (refEntry, null);
    }

    private async Task<HashId> CreateMergeCommit(
        Guid projectId,
        string title,
        string targetRefName,
        string sourceRefName,
        HashId targetTip,
        HashId sourceTip,
        UserIdentity author,
        CancellationToken cancellationToken = default
    )
    {
        var commonAncestor = await commitService.FindCommonAncestorAsync(
            projectId,
            targetTip,
            sourceTip,
            cancellationToken
        );

        var targetSnapshot = await commitService.GetSnapshotAsync(
            projectId,
            targetTip,
            commonAncestor?.Id,
            cancellationToken
        );

        var sourceSnapshot = await commitService.GetSnapshotAsync(
            projectId,
            sourceTip,
            commonAncestor?.Id,
            cancellationToken
        );

        var changes = DiffService.DiffSnapshots(targetSnapshot, sourceSnapshot);

        var mergeCommit = new CommitBuilder()
            .AddParentId(targetTip)
            .AddSecondParentId(sourceTip)
            .AddKind(CommitKind.Merge)
            .AddFileChanges(changes)
            .AddAuthor(author)
            .AddMessage($"Merged {sourceRefName} into {targetRefName}, \"{title}\"")
            .AddCreatedAt(DateTimeOffset.UtcNow)
            .GetCommit();

        await commitRepository.AddAsync(projectId, mergeCommit, cancellationToken);

        return mergeCommit.Id;
    }

    private async Task CreateMergeRequest(
        Guid authorId,
        Guid projectId,
        string title,
        string targetRefName,
        string sourceRefName,
        HashId mergeCommitId,
        CancellationToken cancellationToken = default
    )
    {
        var mergeRequest = MergeRequest.Create(
            authorId,
            projectId,
            title,
            targetRefName,
            sourceRefName,
            mergeCommitId
        );
        await mergeRequestRepository.AddAsync(mergeRequest, cancellationToken);
    }
}