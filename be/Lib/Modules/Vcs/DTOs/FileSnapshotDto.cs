using Core.FileSnapshots;
using Core.Storage;

namespace Lib.Modules.Vcs.DTOs;

public record FileSnapshotDto(
    string FilePath,
    HashId BlobId,
    DateTimeOffset LastModified
)
{
    public static FileSnapshotDto FromDomain(FileSnapshot domain)
    {
        return new FileSnapshotDto(domain.FilePath, domain.BlobId, domain.LastModified);
    }

    public FileSnapshot ToDomain()
    {
        return new FileSnapshot(FilePath, BlobId, LastModified);
    }
}