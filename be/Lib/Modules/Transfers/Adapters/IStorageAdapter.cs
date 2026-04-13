using Lib.Modules.Transfers.Stores;

namespace Lib.Modules.Transfers.Adapters;

public interface IStorageAdapter
{
    public IFullTusStore CreateTusStore(Guid userId, string scopePath);
}