using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Syed.PortfolioPlatform.Application.DTOs.Articles
{
    public class CreateArticleRequest
    {
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? ImageUrl { get; set; } = null!;

        public string? YouTubeLink { get; set; } = null;
        public string? PDFLink { get; set; } = null;

        public IFormFile? FeaturedImage { get; set; }

        // UI DROPDOWNS
        public IEnumerable<SelectListItem> CategoryOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> TagOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();
        public Guid CategoryId { get; set; }

        // SEO
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }

        // Publishing
        public DateTime? PublishedDate { get; set; }
        public bool IsPublished { get; set; }

        // NEW: Tag IDs for many-to-many
        public List<Guid>? TagIds { get; set; } = new List<Guid>();
    }
}
