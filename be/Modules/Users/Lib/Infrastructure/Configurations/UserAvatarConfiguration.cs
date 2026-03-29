using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Lib.Entities;

namespace Users.Lib.Infrastructure.Configurations;

public class UserAvatarConfiguration : IEntityTypeConfiguration<UserAvatar>
{
    public void Configure(EntityTypeBuilder<UserAvatar> builder)
    {
        builder.ToTable("user_avatars");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.Property(a => a.StorageKey)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(a => a.Url)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(a => a.SizeBytes)
            .IsRequired();

        builder.Property(a => a.MimeType)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(a => a.CreatedAt)
            .IsRequired();
    }
}