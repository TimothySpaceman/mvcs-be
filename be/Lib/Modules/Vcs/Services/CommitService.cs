using System.Collections.Immutable;
using System.Runtime.CompilerServices;
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

    public async IAsyncEnumerable<Commit> GetChainAsync(
        Guid projectId,
        HashId toId,
        HashId? fromId = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
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

        while (true)
        {
            yield return current;

            if (current.Id == fromId || current.ParentId is null) break;

            var parentId = current.ParentId.Value;
            if (!allCommits.TryGetValue(parentId, out current!))
            {
                throw new CommitNotFoundException($"Parent commit {parentId} not found");
            }
        }
    }

    public async Task<Snapshot> GetSnapshotAsync(Guid projectId, HashId commitId,
        CancellationToken cancellationToken = default)
    {
        var chain = new List<Commit>();
        await foreach (var commit in GetChainAsync(projectId, commitId, null, cancellationToken))
        {
            chain.Add(commit);
        }

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