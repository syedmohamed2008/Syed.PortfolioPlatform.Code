using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syed.PortfolioPlatform.Domain.Entities;

namespace Syed.PortfolioPlatform.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(150);

            builder.Property(c => c.Slug)
                    .IsRequired()
                    .HasMaxLength(150);

            builder.Property(c => c.Description).HasMaxLength(500);
            builder.Property(c => c.IsDeleted).HasDefaultValue(false);

            // Unique index for Slug
            builder.HasIndex(c => c.Slug).IsUnique();
        }
    }
}
