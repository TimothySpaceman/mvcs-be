using Core.Storage;
using Lib.Modules.Projects.Entities;
using Lib.Modules.Transfers.Adapters;
using Lib.Modules.Transfers.Helpers;
using Lib.Modules.Transfers.Services;
using Lib.Modules.Vcs.Entities;
using Lib.Modules.Vcs.Repository;
using Lib.Shared.Exceptions;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;

namespace Lib.Modules.Vcs.Services;

public class BlobTransferService(
    IBlobMetadataRepository blobMetadataRepository,
    ITransferService transferService
) : IBlobTransferService
{
    public async Task<DefaultTusConfiguration> GetTusConfigurationAsync(Project project, Guid userId)
    {
        var scopedPath = GetScopedPath(project.Id);
        var tusConfig = await transferService.GetTusConfigurationAsync(project.StorageId, userId, scopedPath);
        tusConfig.Events.OnFileCompleteAsync += ctx => OnBlobUploadCompleteAsync(ctx, project.Id);
        return tusConfig;
    }

    private async Task OnBlobUploadCompleteAsync(FileCompleteContext ctx, Guid projectId)
    {
        var store = (ITusCreationStore)ctx.Store;
        var metadata = await store.GetUploadMetadataAsync(ctx.FileId, ctx.CancellationToken);
        var parsed = TusMetadataHelper.ParseMetadata(metadata);

        var blobIdHex = parsed["filename"];
        HashId blobId;
        try
        {
            blobId = new HashId(Convert.FromHexString(blobIdHex));
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("Blob id is not valid hex string", ex);
        }

        var length = long.Parse(parsed["length"]);
        var blobMetadata = BlobMetadataEntity.Create(blobId, projectId, length);
        await blobMetadataRepository.AddAsync(blobMetadata);
        await blobMetadataRepository.SaveChangesAsync();
    }

    public async Task<(Stream content, long length, ByteRange clampedRange)> GetBlobAsync(
        Project project,
        HashId blobId,
        ByteRange? range,
        CancellationToken cancellationToken = default
    )
    {
        var blobMetadata = await blobMetadataRepository.GetByIdAsync(blobId, project.Id);
        if (blobMetadata is null) throw new NotFoundException("Blob not found");

        var rangeStart = range?.Start ?? 0;
        var rangeEnd = range?.End ?? blobMetadata.Length - 1;

        var scopedPath = GetScopedPath(project.Id);
        var blobPath = $"{scopedPath}/{blobId.ToHexString()}";
        var content = await transferService.GetContentAsync(
            project.StorageId,
            blobPath,
            range,
            cancellationToken
        );

        return (content, blobMetadata.Length, new ByteRange(rangeStart, rangeEnd));
    }

    private string GetScopedPath(Guid projectId)
    {
        return $"vcs/{projectId}/blobs";
    }
}