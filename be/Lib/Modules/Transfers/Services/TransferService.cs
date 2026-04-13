using Lib.Modules.Storages.Services;
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
        var storage = await storageService.GetRawByIdAsync(storageId, userId);
        var adapter = adapterFactory.Create(storage);
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
}