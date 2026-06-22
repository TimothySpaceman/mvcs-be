namespace Lib.Modules.Transfers.Utils;

public interface IRemoteFileClient : IAsyncDisposable
{
    public Task ConnectAsync(CancellationToken ct);
    public Task<bool> FileExistsAsync(string path, CancellationToken ct);
    public Task<long> GetFileSizeAsync(string path, CancellationToken ct);
    public Task EnsureDirectoryAsync(string dirPath, CancellationToken ct);
    public Task AppendAsync(string path, Stream data, CancellationToken ct);
    public Task<Stream> OpenReadAsync(string path, long offset, CancellationToken ct);
    public Task DeleteFileAsync(string path, CancellationToken ct);
    public Task RenameAsync(string from, string to, CancellationToken ct);
}