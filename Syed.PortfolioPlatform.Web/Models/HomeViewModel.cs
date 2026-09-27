using Syed.PortfolioPlatform.Application.DTOs.Articles;

namespace Syed.PortfolioPlatform.Web.Models
{
    public class HomeViewModel
    {
        public List<ArticleDto> LatestArticles { get; set; } = new();
    }
}
