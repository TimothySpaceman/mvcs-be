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
}