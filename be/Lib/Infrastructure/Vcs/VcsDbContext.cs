using Lib.Modules.Vcs;
using Microsoft.EntityFrameworkCore;

namespace Lib.Infrastructure.Vcs;

public class VcsDbContext(DbContextOptions<VcsDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyVcsConfigurations();
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ApplyVcsConventions();
    }
}