using Lib.Modules.Tasks.Entities;
using Lib.Modules.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lib.Modules.Tasks.Configurations;

public class TaskAssignmentConfiguration : IEntityTypeConfiguration<TaskAssignment>
{
    public void Configure(EntityTypeBuilder<TaskAssignment> builder)
    {
        builder.ToTable("task_assignments");

        builder.HasKey(a => new { a.TaskId, a.UserId });

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.HasIndex(a => a.UserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}