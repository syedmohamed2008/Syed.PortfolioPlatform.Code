using Syed.PortfolioPlatform.Application.DTOs.Articles;
using Syed.PortfolioPlatform.Application.DTOs.Tags;

namespace Syed.PortfolioPlatform.Web.Models
{
    public class ArticleDetailsViewModel
    {
        public ArticleDto Article { get; set; }

        public List<TagDto> Tags { get; set; } = new();
        public double ReadingTime { get; set; }

        public List<ArticleDto> RelatedArticles { get; set; } = new();
    }
}
