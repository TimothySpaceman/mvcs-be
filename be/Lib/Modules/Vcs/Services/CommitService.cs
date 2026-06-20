using System.Collections.Immutable;
using Core.Commits;
using Core.Exceptions;
using Core.FileChanges;
using Core.FileSnapshots;
using Core.Snapshots;
using Core.Storage;
using Lib.Modules.Vcs.Repository;

namespace Lib.Modules.Vcs.Services;

public class CommitService(ICommitRepository commitRepository) : ICommitService
{
    public Task<Dictionary<HashId, Commit>> GetAllAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return commitRepository.GetAllAsync(projectId, cancellationToken);
    }

    public Task<Commit?> GetAsync(Guid projectId, HashId commitId, CancellationToken cancellationToken = default)
    {
        return commitRepository.GetAsync(projectId, commitId, cancellationToken);
    }

    public Task<IEnumerable<Commit>> GetChainAsync(
        Guid projectId,
        HashId toId,
        HashId? fromId = null,
        CancellationToken cancellationToken = default
    )
    {
        return GetChainAsync(projectId, toId, fromId, null, cancellationToken);
    }

    public async Task<IEnumerable<Commit>> GetChainAsync(
        Guid projectId,
        HashId toId,
        HashId? fromId = null,
        int? limit = null,
        CancellationToken cancellationToken = default
    )
    {
        var allCommits = await commitRepository.GetAllAsync(projectId, cancellationToken);

        if (!allCommits.TryGetValue(toId, out var current))
        {
            throw new CommitNotFoundException($"Target commit {toId} not found");
        }

        if (fromId is not null && !allCommits.ContainsKey(fromId.Value))
        {
            throw new CommitNotFoundException($"Starting commit {fromId} not found");
        }

        var chain = new List<Commit>();
        while (true)
        {
            chain.Add(current);

            if (current.Id == fromId || current.ParentId is null) break;
            if (limit is not null && chain.Count == limit) break;

            var parentId = current.ParentId.Value;
            if (!allCommits.TryGetValue(parentId, out current!))
            {
                throw new CommitNotFoundException($"Parent commit {parentId} not found");
            }
        }

        return chain;
    }

    public async Task<Commit?> FindCommonAncestorAsync(
        Guid projectId,
        HashId idA,
        HashId idB,
        CancellationToken cancellationToken = default
    )
    {
        var allCommits = await commitRepository.GetAllAsync(projectId, cancellationToken);

        var ancestorsA = new HashSet<HashId>();
        var current = allCommits.GetValueOrDefault(idA);
        while (current is not null)
        {
            ancestorsA.Add(current.Id);
            current = current.ParentId is null ? null : allCommits.GetValueOrDefault(current.ParentId.Value);
        }

        current = allCommits.GetValueOrDefault(idB);
        while (current is not null)
        {
            if (ancestorsA.Contains(current.Id)) return current;
            current = current.ParentId is null ? null : allCommits.GetValueOrDefault(current.ParentId.Value);
        }

        return null;
    }

    public async Task<Snapshot> GetSnapshotAsync(
        Guid projectId,
        HashId commitId,
        CancellationToken cancellationToken = default
    )
    {
        return await GetSnapshotAsync(projectId, commitId, null, cancellationToken);
    }

    public async Task<Snapshot> GetSnapshotAsync(
        Guid projectId,
        HashId commitId,
        HashId? fromId = null,
        CancellationToken cancellationToken = default
    )
    {
        var chain = (await GetChainAsync(projectId, commitId, fromId, cancellationToken)).ToList();
        chain.Reverse();

        var files = new Dictionary<string, FileSnapshot>();
        foreach (var commit in chain)
        {
            foreach (var change in commit.Changes)
            {
                ApplyFileChange(files, change);
            }
        }

        return new Snapshot(files.ToImmutableDictionary());
    }

    private static void ApplyFileChange(Dictionary<string, FileSnapshot> files, FileChange change)
    {
        if (change.IsCreation)
        {
            files[change.After!.FilePath] = change.After;
        }
        else if (change.IsRemoval)
        {
            files.Remove(change.Before!.FilePath);
        }
        else
        {
            files.Remove(change.Before!.FilePath);
            files[change.After!.FilePath] = change.After;
        }
    }
}