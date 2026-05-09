using Core.Blobs;
using Lib.Modules.Vcs.Entities;

namespace Lib.Modules.Vcs.Mappings;

public static class BlobMetadataMapping
{
    public static BlobMetadataEntity ToEntity(this BlobMetadata domain, Guid projectId)
    {
        return BlobMetadataEntity.Create(domain.Id, projectId, domain.Length);
    }

    public static BlobMetadata ToDomain(this BlobMetadataEntity entity)
    {
        return new BlobMetadata(entity.Id, entity.Length);
    }
}