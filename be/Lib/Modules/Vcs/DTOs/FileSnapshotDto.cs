using Core.FileSnapshots;
using Core.Storage;

namespace Lib.Modules.Vcs.DTOs;

public record FileSnapshotDto(
    string FilePath,
    HashId BlobId
)
{
    public static FileSnapshotDto FromDomain(FileSnapshot domain)
    {
        return new FileSnapshotDto(domain.FilePath, domain.BlobId);
    }
}