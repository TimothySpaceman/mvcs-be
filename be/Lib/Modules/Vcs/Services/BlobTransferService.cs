using Core.Storage;
using Lib.Modules.Projects.Services;
using Lib.Modules.Transfers.Helpers;
using Lib.Modules.Transfers.Services;
using Lib.Modules.Vcs.Entities;
using Lib.Modules.Vcs.Repository;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;

namespace Lib.Modules.Vcs.Services;

public class BlobTransferService(
    IBlobMetadataRepository blobMetadataRepository,
    IProjectService projectService,
    ITransferService transferService
) : IBlobTransferService
{
    public async Task<DefaultTusConfiguration> GetTusConfigurationAsync(Guid projectId, Guid userId)
    {
        var project = await projectService.GetRawByIdAsync(projectId);
        var scopedPath = $"vcs/{projectId}/blobs";
        var tusConfig = await transferService.GetTusConfigurationAsync(project.StorageId, userId, scopedPath);
        tusConfig.Events.OnFileCompleteAsync += ctx => OnBlobUploadCompleteAsync(ctx, projectId);
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
            throw new InvalidOperationException("Blob filename is not valid hex string", ex);
        }

        var length = long.Parse(parsed["length"]);
        var blobMetadata = BlobMetadataEntity.Create(blobId, projectId, length);
        await blobMetadataRepository.AddAsync(blobMetadata);
        await blobMetadataRepository.SaveChangesAsync();
    }
}