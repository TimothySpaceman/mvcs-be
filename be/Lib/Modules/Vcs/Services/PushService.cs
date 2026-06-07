using Core.Commits;
using Core.Storage;
using Lib.Modules.Projects.Entities;
using Lib.Modules.Projects.Services;
using Lib.Modules.Vcs.Entities;
using Lib.Modules.Vcs.Repository;
using Lib.Shared.Exceptions;

namespace Lib.Modules.Vcs.Services;

public class PushService(
    IBlobMetadataRepository blobMetadataRepository,
    ICommitRepository commitRepository,
    IRefRepository refRepository,
    IProjectService projectService
) : IPushService
{
    public async Task UpdateCommitsChainAsync(
        Guid projectId,
        string refName,
        IEnumerable<Commit> commits,
        CancellationToken cancellationToken = default
    )
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        await UpdateCommitsChainAsync(project, refName, commits, cancellationToken);
    }

    public async Task UpdateCommitsChainAsync(
        Project project,
        string refName,
        IEnumerable<Commit> commits,
        CancellationToken cancellationToken = default
    )
    {
        var commitsList = commits.ToList();
        var latestCommit = GetLatestCommit(commitsList);

        var blobIds = commitsList.SelectMany(c => c.Changes
            .Where(fc => fc.After is not null)
            .Select(fc => fc.After!.BlobId)
        ).Distinct();
        await EnsureBlobIdsAsync(project.Id, blobIds, cancellationToken);

        await commitRepository.AddRangeAsync(project.Id, commitsList, cancellationToken);

        var refEntry = await refRepository.GetAsync(project.Id, refName, cancellationToken);
        if (refEntry is null)
        {
            refEntry = RefEntity.Create(project.Id, refName, latestCommit.Id);
            await refRepository.AddAsync(refEntry, cancellationToken);
        }
        else
        {
            refEntry.SetCommitId(latestCommit.Id);
        }

        await refRepository.SaveChangesAsync(cancellationToken);

        if (!project.IsInitialized) await projectService.InitializeAsync(project, refName);
    }

    private async Task EnsureBlobIdsAsync(
        Guid projectId,
        IEnumerable<HashId> blobIds,
        CancellationToken cancellationToken = default
    )
    {
        var existingIds = (await blobMetadataRepository.GetAllByProjectIdAsync(projectId, cancellationToken))
            .Select(bm => bm.Id)
            .ToHashSet();
        var missingId = blobIds.FirstOrDefault(b => !existingIds.Contains(b));
        if (!missingId.IsEmpty) throw new BadRequestException($"Blob {missingId} not found");
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