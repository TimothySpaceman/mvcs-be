using System.Text;
using System.Text.Json;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Lib.Modules.Storages.Entities;
using Lib.Modules.Storages.Services;
using Lib.Modules.Uploads.ConfigModels;
using Lib.Modules.Uploads.Factories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;

namespace Lib.Modules.Uploads.Services;

public class UploadService(
    IStorageService storageService,
    IStorageAdapterFactory adapterFactory
) : IUploadService
{
    public async Task<DefaultTusConfiguration> GetTusConfigurationAsync(
        Guid storageId,
        Guid userId,
        string scopePath,
        HttpContext httpContext)
    {
        var storage = await storageService.GetRawByIdAsync(storageId, userId);
        var adapter = adapterFactory.Create(storage);
 
        return new DefaultTusConfiguration
        {
            Store = adapter.CreateTusStore(),
            MetadataParsingStrategy = MetadataParsingStrategy.AllowEmptyValues,
            Events = new Events
            {
                OnFileCompleteAsync = async ctx =>
                {
                    var file = await ctx.GetFileAsync();
                    var metadata = await file.GetMetadataAsync(ctx.CancellationToken);
                    
                    var hasName = metadata.TryGetValue("filename", out var rawFileName);
                    var fileName = hasName ? rawFileName!.GetString(Encoding.UTF8) : ctx.FileId; // TODO: sanitize
                    var sourceKey = ctx.FileId;
                    var finalKey = adapter.BuildFinalKey(userId, scopePath, fileName);

                    await adapter.MoveFileAsync(sourceKey, finalKey, ctx.CancellationToken);
                }
            }
        };
    }
}
