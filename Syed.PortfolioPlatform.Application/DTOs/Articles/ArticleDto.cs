using Syed.PortfolioPlatform.Application.DTOs.Categories;
using Syed.PortfolioPlatform.Application.DTOs.Tags;

namespace Syed.PortfolioPlatform.Application.DTOs.Articles
{
    public class ArticleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string? YouTubeLink { get; set; } = null;
        public string? PDFLink { get; set; } = null;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public DateTime? PublishedDate { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public int Views { get; set; }

        public double ReadingTime { get; set; }

        public Guid CreatedByUserId { get; set; }
        public string CreatedByUsername { get; set; } = null!;
        public List<TagDto> Tags { get; set; } = new();

        public List<Guid> TagIds { get; set; } = new();
        public DateTime CreatedOn { get; set; }   

        




    }

}
