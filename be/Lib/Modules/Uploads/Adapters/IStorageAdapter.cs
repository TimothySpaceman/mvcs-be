using Lib.Modules.Uploads.Stores;

namespace Lib.Modules.Uploads.Adapters;

public interface IStorageAdapter
{
    public IFullTusStore CreateTusStore(Guid userId, string scopePath);
}