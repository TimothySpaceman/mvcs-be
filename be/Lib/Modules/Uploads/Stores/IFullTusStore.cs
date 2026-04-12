using tusdotnet.Interfaces;

namespace Lib.Modules.Uploads.Stores;

public interface IFullTusStore : ITusPipelineStore, ITusCreationStore, ITusTerminationStore, ITusCompletionStore
{
}