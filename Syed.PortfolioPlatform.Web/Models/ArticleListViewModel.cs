using Syed.PortfolioPlatform.Application.DTOs.Articles;
using Syed.PortfolioPlatform.Application.DTOs.Categories;
using Syed.PortfolioPlatform.Application.DTOs.Tags;

namespace Syed.PortfolioPlatform.Web.ViewModels.Articles
{
    public class ArticleListViewModel
    {
        public List<ArticleDto> Articles { get; set; } = new();

        public List<CategoryDto> Categories { get; set; } = new();

        public List<TagDto> Tags { get; set; } = new();

        public string? Search { get; set; }

        public Guid? CategoryId { get; set; }

        public Guid? TagId { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}