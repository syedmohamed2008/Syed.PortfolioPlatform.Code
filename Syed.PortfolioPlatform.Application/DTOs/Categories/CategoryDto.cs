namespace Syed.PortfolioPlatform.Application.DTOs.Categories
{
    public class CategoryDto
    {
        public Guid Id { get; set; }            // From BaseEntity
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
        public int ArticleCount { get; set; }   // Optional: number of articles
    }
}
