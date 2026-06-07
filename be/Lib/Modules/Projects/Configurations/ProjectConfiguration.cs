using Lib.Modules.Projects.Entities;
using Lib.Modules.Storages.Entities;
using Lib.Modules.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lib.Modules.Projects.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(p => p.Description)
            .HasMaxLength(1024);

        builder.Property(p => p.IsPublic)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.IsInitialized)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(p => p.DefaultRefName)
            .HasMaxLength(255);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        builder.Ignore(p => p.IsDeleted);

        builder.HasIndex(u => u.AuthorId);
        builder.HasIndex(u => u.StorageId);
        builder.HasIndex(u => u.Title);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Storage>()
            .WithMany()
            .HasForeignKey(c => c.StorageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.AccessEntries)
            .WithOne(a => a.Project)
            .HasForeignKey(a => a.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(p => p.DeletedAt == null);
    }
}