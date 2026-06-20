using Lib.Modules.Storages.Entities;
using Lib.Modules.Storages.Services;
using Lib.Modules.Transfers.Adapters;
using Lib.Modules.Transfers.DTOs;
using Lib.Modules.Transfers.Factories;
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
        string scopePath
    )
    {
        var adapter = await GetStorageAdapter(storageId);
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
        string filePath,
        CancellationToken cancellationToken = default
    )
    {
        var adapter = await GetStorageAdapter(storageId);
        return await adapter.GetContentLengthAsync(filePath, cancellationToken);
    }

    public async Task<Stream> GetContentAsync(
        Guid storageId,
        string filePath,
        ByteRange? range,
        CancellationToken cancellationToken = default
    )
    {
        var adapter = await GetStorageAdapter(storageId);
        return await adapter.GetContentAsync(filePath, range, cancellationToken);
    }

    public async Task<StorageHealthDto> GetStorageHealthAsync(
        Guid storageId,
        CancellationToken cancellationToken = default
    )
    {
        var adapter = await GetStorageAdapter(storageId);
        return await adapter.GetStorageHealthAsync(cancellationToken);
    }
    
    public async Task<StorageHealthDto> GetStorageHealthAsync(
        Storage storage,
        CancellationToken cancellationToken = default
    )
    {
        var adapter = GetStorageAdapter(storage);
        return await adapter.GetStorageHealthAsync(cancellationToken);
    }

    private async Task<IStorageAdapter> GetStorageAdapter(Guid storageId)
    {
        var storage = await storageService.GetRawByIdAsync(storageId);
        return GetStorageAdapter(storage);
    }

    private IStorageAdapter GetStorageAdapter(Storage storage)
    {
        return adapterFactory.Create(storage);
    }
}