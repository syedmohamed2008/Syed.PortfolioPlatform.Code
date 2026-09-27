namespace Syed.PortfolioPlatform.Application.DTOs.Tags
{
    public class TagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public int ArticleCount { get; set; }   // Optional
    }
}
