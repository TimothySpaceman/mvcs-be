namespace Lib.Modules.Vcs.Entities;

public class BlobMetadataEntity
{
    public byte[] Id { get; private set; } = null!;
    public Guid ProjectId { get; private set; }
    public long Length { get; private set; }

    private BlobMetadataEntity()
    {
    }

    public static BlobMetadataEntity Create(byte[] id, Guid projectId, long length)
    {
        return new BlobMetadataEntity
        {
            Id = id,
            Length = length,
            ProjectId = projectId
        };
    }
}