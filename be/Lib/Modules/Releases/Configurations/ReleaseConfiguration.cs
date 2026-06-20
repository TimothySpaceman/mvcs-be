using Lib.Modules.Projects.Entities;
using Lib.Modules.Releases.Entities;
using Lib.Modules.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lib.Modules.Releases.Configurations;

public class ReleaseConfiguration : IEntityTypeConfiguration<Release>
{
    public void Configure(EntityTypeBuilder<Release> builder)
    {
        builder.ToTable("releases");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(r=> r.ProjectId);
        builder.Property(r=> r.AuthorId);
        
        builder.Property(r=> r.CreatedAt)
            .IsRequired();
        
        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(r=> r.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r=> r.AuthorId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasMany(t => t.Files)
            .WithOne()
            .HasForeignKey(a => a.ReleaseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(r=> r.ProjectId);
    }
}