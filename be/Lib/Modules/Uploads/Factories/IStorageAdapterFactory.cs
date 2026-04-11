using Lib.Modules.Storages.Entities;
using Lib.Modules.Uploads.Adapters;

namespace Lib.Modules.Uploads.Factories;

public interface IStorageAdapterFactory
{
    IStorageAdapter Create(Storage storage);
}