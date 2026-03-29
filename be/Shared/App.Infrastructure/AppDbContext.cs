using Microsoft.EntityFrameworkCore;
using Users.Lib.Entities;

namespace App.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(User).Assembly);
    }
}