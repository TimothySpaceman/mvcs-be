using Lib.Modules.Auth;
using Lib.Modules.Projects;
using Lib.Modules.Storages;
using Lib.Modules.Tasks;
using Lib.Modules.Users;
using Microsoft.EntityFrameworkCore;

namespace Lib.Infrastructure.App;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyUsersConfigurations();
        modelBuilder.ApplyAuthConfigurations();
        modelBuilder.ApplyStoragesConfigurations();
        modelBuilder.ApplyProjectsConfigurations();
        modelBuilder.ApplyTasksConfigurations();
    }
}