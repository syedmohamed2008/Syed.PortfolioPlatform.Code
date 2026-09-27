using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syed.PortfolioPlatform.Domain.Entities;

namespace Syed.PortfolioPlatform.Infrastructure.Persistence.Configurations
{
    public class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.ToTable("Articles");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(a => a.Slug)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(a => a.Content)
                   .IsRequired();

            builder.Property(a => a.ImageUrl)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(a => a.IsDeleted)
                   .HasDefaultValue(false);

            builder.Property(a => a.IsPublished)
                   .HasDefaultValue(false);

            builder.Property(a => a.Views)
                   .HasDefaultValue(0);

            // Relationships
            builder.HasOne(a => a.Category)
                   .WithMany(c => c.Articles)
                   .HasForeignKey(a => a.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.CreatedByUser)
                   .WithMany(u => u.Articles)
                   .HasForeignKey(a => a.CreatedByUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(a => a.Slug).IsUnique();
            builder.HasIndex(a => a.CategoryId);
            builder.HasIndex(a => new { a.IsPublished, a.IsDeleted });
            builder.HasIndex(a => a.CreatedOn);
        }
    }
}
