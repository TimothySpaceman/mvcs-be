using Microsoft.AspNetCore.Http;
using tusdotnet.Models;

namespace Lib.Modules.Uploads.Services;

public interface IUploadService
{
    Task<DefaultTusConfiguration> GetTusConfigurationAsync(
        Guid storageId,
        Guid userId,
        string scopePath,
        HttpContext httpContext
    );
}
