using FluentFTP;
using Lib.Modules.Transfers.ConfigModels;
using Lib.Modules.Transfers.Utils;

namespace Lib.Modules.Transfers.Adapters;

public class FtpRemoteFileClient : IRemoteFileClient
{
    private readonly FtpStorageConfig _config;
    private AsyncFtpClient? _client;

    public FtpRemoteFileClient(FtpStorageConfig config)
    {
        _config = config;
    }

    private AsyncFtpClient Client => _client ?? throw new InvalidOperationException("Not connected");

    public async Task ConnectAsync(CancellationToken ct)
    {
        var encryption = _config.FtpEncryption.Trim().ToLowerInvariant() switch
        {
            "none" => FtpEncryptionMode.None,
            "explicit" => FtpEncryptionMode.Explicit,
            "implicit" => FtpEncryptionMode.Implicit,
            _ => FtpEncryptionMode.Auto
        };
        
        _client = new AsyncFtpClient(
            _config.Host,
            _config.Username,
            _config.Password ?? string.Empty,
            _config.Port
        );
        _client.Config.EncryptionMode = encryption;
        _client.Config.ValidateAnyCertificate = !_config.FtpValidateCertificate;

        await _client.Connect(ct);
    }

    public Task<bool> FileExistsAsync(string path, CancellationToken ct) => Client.FileExists(path, ct);

    public async Task<long> GetFileSizeAsync(string path, CancellationToken ct)
    {
        var size = await Client.GetFileSize(path, -1, ct);
        return size;
    }

    public Task EnsureDirectoryAsync(string dirPath, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(dirPath)) return Task.CompletedTask;
        return Client.CreateDirectory(dirPath, true, ct);
    }

    public async Task AppendAsync(string path, Stream data, CancellationToken ct)
    {
        await using var remote = await Client.OpenAppend(path, FtpDataType.Binary, -1, ct);
        await data.CopyToAsync(remote, ct);
        await remote.FlushAsync(ct);
    }

    public async Task<Stream> OpenReadAsync(string path, long offset, CancellationToken ct)
    {
        return await Client.OpenRead(path, FtpDataType.Binary, offset, true, ct);
    }

    public Task DeleteFileAsync(string path, CancellationToken ct) => Client.DeleteFile(path, ct);

    public async Task RenameAsync(string from, string to, CancellationToken ct)
    {
        await Client.MoveFile(from, to, FtpRemoteExists.Overwrite, ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_client is not null)
        {
            if (_client.IsConnected) await _client.Disconnect();
            _client.Dispose();
            _client = null;
        }
    }
}