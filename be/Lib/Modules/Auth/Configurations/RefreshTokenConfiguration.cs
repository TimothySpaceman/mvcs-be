using Lib.Modules.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lib.Modules.Auth.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        
        builder.HasKey(r => r.Id);

        builder.Property(r => r.TokenHash)
            .HasMaxLength(128)
            .IsRequired();
        
        builder.Property(r => r.CreatedAt)
            .IsRequired();
        
        builder.Property(r => r.ExpiresAt)
            .IsRequired();
        
        builder.HasOne(r => r.Session)
            .WithOne(s => s.RefreshToken)
            .HasForeignKey<RefreshToken>(r => r.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(r => r.SessionId).IsUnique();
    }
}