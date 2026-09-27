using Syed.PortfolioPlatform.Domain.Common;

namespace Syed.PortfolioPlatform.Domain.Entities
{
    public class Article : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;

        public string? YouTubeLink { get; set; }

        public string? PDFLink { get; set; }

        // Foreign key
        public Guid CategoryId { get; set; }
        // Navigation property
        public Category Category { get; set; } = default!;

        // SEO
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }

        // Publishing
        public DateTime? PublishedDate { get; set; }

        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }

        public int Views { get; set; }

     

        // Ownership
        public Guid CreatedByUserId { get; set; }
        public AdminUser CreatedByUser { get; set; } = null!;

        // Many-to-many with Tags
        public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
    }
}
