using Core.Commits;
using Core.FileChanges;
using Core.FileSnapshots;
using Core.Storage;

namespace Lib.Modules.Vcs.Helpers;

public static class BlobHelper
{
    public static HashId GetBlobFromFileSnapshot(FileSnapshot fileSnapshot)
    {
        return fileSnapshot.BlobId;
    }

    public static IEnumerable<HashId> GetBlobsFromFileChange(FileChange fileChange)
    {
        var result = new List<HashId>();
        if(fileChange.Before is not null) result.Add(GetBlobFromFileSnapshot(fileChange.Before));
        if(fileChange.After is not null) result.Add(GetBlobFromFileSnapshot(fileChange.After));
        return result;
    }

    public static IEnumerable<HashId> GetBlobsFromCommit(Commit commit)
    {
        return commit.Changes.SelectMany(GetBlobsFromFileChange);
    }
    
    public static IEnumerable<HashId> GetBlobsFromCommitsChain(IEnumerable<Commit> commits)
    {
        return commits.SelectMany(GetBlobsFromCommit);
    }
}