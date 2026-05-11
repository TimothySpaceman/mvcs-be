using Core.Commits;
using Lib.Infrastructure.Vcs;
using Lib.Modules.Vcs.Repository;

namespace Lib.Modules.Vcs.Factories;

public class CommitStoreFactory(VcsDbContext db) : ICommitStoreFactory
{
    public ICommitStore Create(Guid projectId)
    {
        return new CommitRepository(db, projectId);
    }
}