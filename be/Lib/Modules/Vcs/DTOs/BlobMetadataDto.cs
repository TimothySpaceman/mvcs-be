using Core.Blobs;
using Core.Storage;

namespace Lib.Modules.Vcs.DTOs;

public record BlobMetadataDto(HashId Id, long Length)
{
    public static BlobMetadataDto FromDomain(BlobMetadata domain)
    {
        return new BlobMetadataDto(
            domain.Id,
            domain.Length
        );
    }
}