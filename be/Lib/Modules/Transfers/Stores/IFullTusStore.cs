using tusdotnet.Interfaces;

namespace Lib.Modules.Transfers.Stores;

public interface IFullTusStore : ITusPipelineStore, ITusCreationStore, ITusTerminationStore, ITusCompletionStore
{
}