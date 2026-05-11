using Core.Commits;

namespace Lib.Modules.Vcs.Factories;

public interface ICommitStoreFactory
{
    ICommitStore Create(Guid projectId);
}