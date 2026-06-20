using System.Data;
using Core.Commits;
using Core.Storage;
using Lib.Infrastructure.Vcs;
using Lib.Modules.Projects.Entities;
using Lib.Modules.Projects.Services;
using Lib.Modules.Vcs.Entities;
using Lib.Modules.Vcs.Repository;
using Lib.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Vcs.Services;

public class PushService(
    IBlobMetadataRepository blobMetadataRepository,
    ICommitRepository commitRepository,
    IRefRepository refRepository,
    IProjectService projectService,
    VcsDbContext vcsDbContext
) : IPushService
{
    public async Task<PushResult> ApplyPushAsync(
        Guid projectId,
        string refName,
        HashId? expectedHead,
        IEnumerable<Commit> commits,
        CancellationToken cancellationToken = default
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        return await ApplyPushAsync(project, refName, expectedHead, commits, cancellationToken);
    }

    public async Task<PushResult> ApplyPushAsync(
        Project project,
        string refName,
        HashId? expectedHead,
        IEnumerable<Commit> commits,
        CancellationToken cancellationToken = default
    )
    {
        var commitsList = commits.ToList();
        if (commitsList.Count == 0)
        {
            throw new BadRequestException("Empty commits chain provided");
        }

        await using var tx = await vcsDbContext.Database.BeginTransactionAsync(
            IsolationLevel.RepeatableRead,
            cancellationToken
        );

        var refEntry = await refRepository.GetForUpdateAsync(project.Id, refName, cancellationToken);
        if (refEntry?.CommitId != expectedHead) return PushResult.RefMismatch;

        var latestCommit = GetLatestCommit(commitsList);

        await PersistCommitsAsync(project.Id, commitsList, cancellationToken);
        await UpdateRefAsync(project.Id, refName, refEntry, latestCommit.Id, cancellationToken);

        await tx.CommitAsync(cancellationToken);

        if (!project.IsInitialized) await projectService.InitializeAsync(project, refName);
        return PushResult.Success;
    }

    private async Task PersistCommitsAsync(
        Guid projectId,
        List<Commit> commits,
        CancellationToken cancellationToken = default
    )
    {
        var blobIds = commits.SelectMany(c => c.Changes
            .Where(fc => fc.After is not null)
            .Select(fc => fc.After!.BlobId)
        ).Distinct();

        await EnsureBlobIdsAsync(projectId, blobIds, cancellationToken);

        await commitRepository.AddRangeAsync(projectId, commits, cancellationToken);
    }

    private async Task UpdateRefAsync(
        Guid projectId,
        string refName,
        RefEntity? refEntry,
        HashId commitId,
        CancellationToken cancellationToken = default
    )
    {
        if (refEntry is null)
        {
            refEntry = RefEntity.Create(projectId, refName, commitId);
            await refRepository.AddAsync(refEntry, cancellationToken);
        }
        else
        {
            refEntry.SetCommitId(commitId);
        }

        await refRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureBlobIdsAsync(
        Guid projectId,
        IEnumerable<HashId> blobIds,
        CancellationToken cancellationToken = default
    )
    {
        var blobIdsList = blobIds.ToList();
        var existingIds = (await blobMetadataRepository.GetAllByIdsAsync(
                blobIdsList,
                projectId,
                cancellationToken
            ))
            .Select(bm => bm.Id)
            .ToHashSet();

        var missingId = blobIdsList
            .Cast<HashId?>()
            .FirstOrDefault(b => !existingIds.Contains(b!.Value));
        if (missingId is not null) throw new BadRequestException($"Blob {missingId} not found");
    }

    private static Commit GetLatestCommit(IReadOnlyList<Commit> commits)
    {
        var ids = commits.Select(c => c.Id).ToHashSet();

        var rootsCount = commits.Count(c => c.ParentId is null || !ids.Contains(c.ParentId.Value));
        if (rootsCount != 1) throw new BadRequestException("Invalid commit chain");

        var parentIds = commits.Where(c => c.ParentId is not null)
            .Select(c => c.ParentId!.Value)
            .ToHashSet();

        var latest = commits.FirstOrDefault(c => !parentIds.Contains(c.Id));
        if (latest is null) throw new BadRequestException("Invalid commit chain");
        return latest;
    }
}