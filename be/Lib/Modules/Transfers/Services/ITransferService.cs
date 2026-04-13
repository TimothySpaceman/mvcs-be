using Lib.Modules.Transfers.Adapters;
using Microsoft.AspNetCore.Http;
using tusdotnet.Models;

namespace Lib.Modules.Transfers.Services;

public interface ITransferService
{
    public Task<DefaultTusConfiguration> GetTusConfigurationAsync(
        Guid storageId,
        Guid userId,
        string scopePath,
        HttpContext httpContext
    );

    public Task<long> GetContentLengthAsync(
        Guid storageId,
        Guid userId,
        string filePath,
        CancellationToken cancellationToken = default
    );

    public Task<Stream> GetContentAsync(
        Guid storageId,
        Guid userId,
        string filePath,
        ByteRange? range,
        CancellationToken cancellationToken = default
    );
}