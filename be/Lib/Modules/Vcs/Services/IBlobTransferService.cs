using Core.Storage;
using Lib.Modules.Projects.Entities;
using Lib.Modules.Transfers.Adapters;
using tusdotnet.Models;

namespace Lib.Modules.Vcs.Services;

public interface IBlobTransferService
{
    public Task<DefaultTusConfiguration> GetTusConfigurationAsync(Project project, Guid userId);

    public Task<(Stream content, long length, ByteRange clampedRange)> GetBlobAsync(
        Project project,
        HashId blobId,
        ByteRange? range,
        CancellationToken cancellationToken = default
    );
}