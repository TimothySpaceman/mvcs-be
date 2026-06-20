using Lib.Modules.Projects.Entities;
using Lib.Modules.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lib.Modules.Projects.Configurations;

public class ProjectAccessConfiguration : IEntityTypeConfiguration<ProjectAccess>
{
    public void Configure(EntityTypeBuilder<ProjectAccess> builder)
    {
        builder.ToTable("project_access");

        builder.HasKey(a => new { a.ProjectId, a.UserId });

        builder.Property(a => a.AccessType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired();
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(a => a.UserId);
    }
}