using tusdotnet.Interfaces;

namespace Lib.Modules.Uploads.Adapters;

public interface IStorageAdapter
{
    ITusStore CreateTusStore();
    string BuildFinalKey(Guid userId, string scopePath, string fileName);
    Task MoveFileAsync(string tempKey, string finalKey, CancellationToken cancellationToken);
}
