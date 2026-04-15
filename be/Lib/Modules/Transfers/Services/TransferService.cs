using Lib.Modules.Storages.Services;
using Lib.Modules.Transfers.Adapters;
using Lib.Modules.Transfers.Factories;
using Microsoft.AspNetCore.Http;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;

namespace Lib.Modules.Transfers.Services;

public class TransferService(
    IStorageService storageService,
    IStorageAdapterFactory adapterFactory
) : ITransferService
{
    public async Task<DefaultTusConfiguration> GetTusConfigurationAsync(
        Guid storageId,
        Guid userId,
        string scopePath,
        HttpContext httpContext)
    {
        var adapter = await GetStorageAdapter(storageId, userId);
        var store = adapter.CreateTusStore(userId, scopePath);

        return new DefaultTusConfiguration
        {
            Store = store,
            MetadataParsingStrategy = MetadataParsingStrategy.AllowEmptyValues,
            Events = new Events
            {
                OnFileCompleteAsync = store.CompleteUploadAsync
            }
        };
    }

    public async Task<long> GetContentLengthAsync(
        Guid storageId,
        Guid userId,
        string filePath,
        CancellationToken cancellationToken = default
    )
    {
        var adapter = await GetStorageAdapter(storageId, userId);
        return await adapter.GetContentLengthAsync(filePath, cancellationToken);
    }

    public async Task<Stream> GetContentAsync(
        Guid storageId,
        Guid userId,
        string filePath,
        ByteRange? range,
        CancellationToken cancellationToken = default
    )
    {
        var adapter = await GetStorageAdapter(storageId, userId);
        return await adapter.GetContentAsync(filePath, range, cancellationToken);
    }

    private async Task<IStorageAdapter> GetStorageAdapter(Guid storageId, Guid userId)
    {
        var storage = await storageService.GetRawByIdAsync(storageId, userId);
        return adapterFactory.Create(storage);
    }
}