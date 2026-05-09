using Core.Storage;

namespace Lib.Modules.Vcs.Entities;

public class BlobMetadataEntity
{
    public HashId Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public long Length { get; private set; }

    private BlobMetadataEntity()
    {
    }

    public static BlobMetadataEntity Create(HashId id, Guid projectId, long length)
    {
        return new BlobMetadataEntity
        {
            Id = id,
            Length = length,
            ProjectId = projectId
        };
    }
}