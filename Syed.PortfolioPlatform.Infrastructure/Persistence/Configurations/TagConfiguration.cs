using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syed.PortfolioPlatform.Domain.Entities;

namespace Syed.PortfolioPlatform.Infrastructure.Persistence.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("Tags");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(t => t.Slug)
                   .IsRequired()
                   .HasMaxLength(100);

            // Unique index for Slug
            builder.HasIndex(t => t.Slug).IsUnique();
        }
    }
}
