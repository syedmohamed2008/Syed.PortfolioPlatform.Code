using Syed.PortfolioPlatform.Domain.Common;

namespace Syed.PortfolioPlatform.Domain.Entities
{
    public class Category : BaseEntity
    {

        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }

        // One-to-many relationship: Category → Articles
        public ICollection<Article> Articles { get; set; } = new List<Article>();

    }
}
