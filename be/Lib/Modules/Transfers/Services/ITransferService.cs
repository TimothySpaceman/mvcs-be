using Lib.Modules.Transfers.Adapters;
using tusdotnet.Models;

namespace Lib.Modules.Transfers.Services;

public interface ITransferService
{
    public Task<DefaultTusConfiguration> GetTusConfigurationAsync(
        Guid storageId,
        Guid userId,
        string scopePath
    );

    public Task<long> GetContentLengthAsync(
        Guid storageId,
        string filePath,
        CancellationToken cancellationToken = default
    );

    public Task<Stream> GetContentAsync(
        Guid storageId,
        string filePath,
        ByteRange? range,
        CancellationToken cancellationToken = default
    );
}