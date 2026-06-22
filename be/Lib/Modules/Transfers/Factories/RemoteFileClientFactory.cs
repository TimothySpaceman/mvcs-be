using Lib.Modules.Transfers.Adapters;
using Lib.Modules.Transfers.ConfigModels;
using Lib.Modules.Transfers.Utils;

namespace Lib.Modules.Transfers.Factories;

public static class RemoteFileClientFactory
{
    public static IRemoteFileClient Create(FtpStorageConfig config)
    {
        if (config.Ssh) return new SftpRemoteFileClient(config);
        return new FtpRemoteFileClient(config);
    }
}