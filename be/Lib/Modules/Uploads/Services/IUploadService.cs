using Microsoft.AspNetCore.Http;
using tusdotnet.Models;

namespace Lib.Modules.Uploads.Services;

public interface IUploadService
{
    public Task<DefaultTusConfiguration> GetTusConfigurationAsync(
        Guid storageId,
        Guid userId,
        string scopePath,
        HttpContext httpContext
    );
}