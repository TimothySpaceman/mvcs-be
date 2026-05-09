using tusdotnet.Models;

namespace Lib.Modules.Vcs.Services;

public interface IBlobTransferService
{
    public Task<DefaultTusConfiguration> GetTusConfigurationAsync(Guid projectId, Guid userId);
}