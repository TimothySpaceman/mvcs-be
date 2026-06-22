using Lib.Modules.Transfers.ConfigModels;
using Lib.Modules.Transfers.Utils;
using Renci.SshNet;

namespace Lib.Modules.Transfers.Adapters;

public class SftpRemoteFileClient : IRemoteFileClient
{
    private readonly FtpStorageConfig _config;
    private SftpClient? _client;

    public SftpRemoteFileClient(FtpStorageConfig config)
    {
        _config = config;
    }

    private SftpClient Client => _client ?? throw new InvalidOperationException("Not connected");

    public Task ConnectAsync(CancellationToken ct)
    {
        var connectionInfo = BuildConnectionInfo();
        _client = new SftpClient(connectionInfo);
        return Task.Run(() => _client.Connect(), ct);
    }

    private ConnectionInfo BuildConnectionInfo()
    {
        var methods = new List<AuthenticationMethod>();

        if (!string.IsNullOrEmpty(_config.PrivateKey))
        {
            using var keyStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(_config.PrivateKey));
            var keyFile = string.IsNullOrEmpty(_config.PrivateKeyPassphrase)
                ? new PrivateKeyFile(keyStream)
                : new PrivateKeyFile(keyStream, _config.PrivateKeyPassphrase);
            methods.Add(new PrivateKeyAuthenticationMethod(_config.Username, keyFile));
        }

        if (!string.IsNullOrEmpty(_config.Password))
        {
            methods.Add(new PasswordAuthenticationMethod(_config.Username, _config.Password));
        }

        if (methods.Count == 0)
        {
            throw new InvalidOperationException("SFTP requires a password or a private key");
        }

        return new ConnectionInfo(_config.Host, _config.Port, _config.Username, methods.ToArray());
    }

    public Task<bool> FileExistsAsync(string path, CancellationToken ct) 
    {
        return Task.Run(() => Client.Exists(path), ct);
    }

    public Task<long> GetFileSizeAsync(string path, CancellationToken ct) 
    {
        return Task.Run(() => Client.Exists(path) ? Client.GetAttributes(path).Size : -1L, ct);
    }

    public Task EnsureDirectoryAsync(string dirPath, CancellationToken ct)
    {
        return Task.Run(() =>
        {
            if (string.IsNullOrEmpty(dirPath) || dirPath == "/") return;

            var segments = dirPath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
            var current = dirPath.StartsWith('/') ? "/" : "";
            foreach (var segment in segments)
            {
                current = current.Length is 0 or 1 && current is "" or "/"
                    ? current + segment
                    : current + "/" + segment;
                if (!Client.Exists(current)) Client.CreateDirectory(current);
            }
        }, ct);
    }

    public async Task AppendAsync(string path, Stream data, CancellationToken ct)
    {
        await using var remote = Client.Open(path, FileMode.Append, FileAccess.Write);
        await data.CopyToAsync(remote, ct);
        await remote.FlushAsync(ct);
    }

    public Task<Stream> OpenReadAsync(string path, long offset, CancellationToken ct)
    {
        return Task.Run(() =>
        {
            var stream = Client.OpenRead(path);
            if (offset > 0) stream.Seek(offset, SeekOrigin.Begin);
            return (Stream)stream;
        }, ct);
    }

    public Task DeleteFileAsync(string path, CancellationToken ct)
    {
        return Task.Run(() =>
        {
            if (Client.Exists(path)) Client.DeleteFile(path);
        }, ct);
    }

    public Task RenameAsync(string from, string to, CancellationToken ct)
    {
        return Task.Run(() =>
        {
            if (Client.Exists(to)) Client.DeleteFile(to);
            Client.RenameFile(from, to);
        }, ct);
    }

    public ValueTask DisposeAsync()
    {
        if (_client is not null)
        {
            if (_client.IsConnected) _client.Disconnect();
            _client.Dispose();
            _client = null;
        }

        return ValueTask.CompletedTask;
    }
}