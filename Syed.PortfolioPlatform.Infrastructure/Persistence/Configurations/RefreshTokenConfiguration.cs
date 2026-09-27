using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syed.PortfolioPlatform.Domain.Entities;

namespace Syed.PortfolioPlatform.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Token)
                   .IsRequired();

            builder.HasOne(rt => rt.Admin)
                   .WithMany(a => a.RefreshTokens)
                   .HasForeignKey(rt => rt.AdminId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
