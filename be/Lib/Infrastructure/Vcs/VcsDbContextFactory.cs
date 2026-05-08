using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Lib.Infrastructure.Vcs;

public class VcsDbContextFactory : IDesignTimeDbContextFactory<VcsDbContext>
{
    public VcsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<VcsDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=vcsdb;Username=app;Password=secret")
            .Options;

        return new VcsDbContext(options);
    }
}