using Lib.Modules.Storages.Entities;
using Lib.Modules.Transfers.Adapters;

namespace Lib.Modules.Transfers.Factories;

public interface IStorageAdapterFactory
{
    public IStorageAdapter Create(Storage storage);
}