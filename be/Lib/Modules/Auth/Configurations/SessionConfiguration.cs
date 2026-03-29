using Lib.Modules.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lib.Modules.Auth.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.IpAddress)
            .HasMaxLength(64)
            .IsRequired();
        
        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.LastActiveAt)
            .IsRequired();
        
        builder.Property(c => c.ExpiresAt)
            .IsRequired();
        
        builder.OwnsOne(s => s.DeviceInfo, device =>
        {
            device.Property(d => d.UserAgent)
                .HasColumnName("device_user_agent")
                .HasMaxLength(512);

            device.Property(d => d.Device)
                .HasColumnName("device_name")
                .HasMaxLength(128);

            device.Property(d => d.OS)
                .HasColumnName("device_os")
                .HasMaxLength(128);

            device.Property(d => d.Browser)
                .HasColumnName("device_browser")
                .HasMaxLength(128);
        });
        
        builder.HasOne(c => c.User)
            .WithOne()
            .HasForeignKey<Session>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.UserId);
    }
}