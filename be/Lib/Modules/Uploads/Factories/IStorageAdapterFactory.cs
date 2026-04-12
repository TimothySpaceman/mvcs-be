using Lib.Modules.Storages.Entities;
using Lib.Modules.Uploads.Adapters;

namespace Lib.Modules.Uploads.Factories;

public interface IStorageAdapterFactory
{
    public IStorageAdapter Create(Storage storage);
}