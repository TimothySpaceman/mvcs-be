using tusdotnet.Models.Configuration;

namespace Lib.Modules.Transfers.Stores;

public interface ITusCompletionStore
{
    public Task CompleteUploadAsync(FileCompleteContext ctx);
}