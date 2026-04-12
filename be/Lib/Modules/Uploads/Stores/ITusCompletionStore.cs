using tusdotnet.Models.Configuration;

namespace Lib.Modules.Uploads.Stores;

public interface ITusCompletionStore
{
    public Task CompleteUploadAsync(FileCompleteContext ctx);
}