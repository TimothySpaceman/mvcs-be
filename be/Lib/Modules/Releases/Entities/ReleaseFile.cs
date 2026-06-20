namespace Lib.Modules.Releases.Entities;

public class ReleaseFile
{
    public Guid Id { get; private set; }
    public Guid ReleaseId { get; private set; }
    public string FileName { get; private set; } = null!;
    public string BlobId { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    private ReleaseFile()
    {
    }

    public static ReleaseFile Create(Guid releaseId, string fileName, string blobId)
    {
        return new ReleaseFile
        {
            Id = Guid.NewGuid(),
            ReleaseId = releaseId,
            FileName = fileName,
            BlobId = blobId,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}