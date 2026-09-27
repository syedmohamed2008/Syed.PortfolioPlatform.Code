using Syed.PortfolioPlatform.Domain.Common;

namespace Syed.PortfolioPlatform.Domain.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;

        // Optional: soft delete
        public bool IsDeleted { get; set; }


        // Many-to-many: Tag -> ArticleTag
        public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
    }
}
