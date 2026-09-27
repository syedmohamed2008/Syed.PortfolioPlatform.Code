using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syed.PortfolioPlatform.Domain.Entities;

namespace Syed.PortfolioPlatform.Infrastructure.Persistence.Configurations
{
    public class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
    {
        public void Configure(EntityTypeBuilder<AdminUser> builder)
        {
            builder.ToTable("AdminUsers");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username).IsRequired().HasMaxLength(150);
            builder.Property(u => u.PasswordHash).IsRequired();

            // One-to-many: AdminUser → Role
            builder.HasOne(u => u.RoleNavigation)
                   .WithMany(r => r.AdminUsers)
                   .HasForeignKey(u => u.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Index for Username
            builder.HasIndex(u => u.Username).IsUnique();
        }
    }
}
