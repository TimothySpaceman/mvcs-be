using Lib.Modules.Transfers.DTOs;
using Lib.Modules.Transfers.Stores;

namespace Lib.Modules.Transfers.Adapters;

public interface IStorageAdapter
{
    public IFullTusStore CreateTusStore(Guid userId, string scopePath);

    public Task<long> GetContentLengthAsync(
        string filePath,
        CancellationToken cancellationToken = default
    );

    public Task<Stream> GetContentAsync(
        string filePath,
        ByteRange? range,
        CancellationToken cancellationToken = default
    );

    public Task<StorageHealthDto> GetStorageHealthAsync(CancellationToken cancellationToken = default);
}