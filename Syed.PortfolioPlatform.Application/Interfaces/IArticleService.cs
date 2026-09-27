using Syed.PortfolioPlatform.Application.Common;
using Syed.PortfolioPlatform.Application.DTOs.Articles;
using Syed.PortfolioPlatform.Application.DTOs.Categories;
using Syed.PortfolioPlatform.Application.DTOs.Tags;

public interface IArticleService
{
    Task<ArticleDto> CreateAsync(CreateArticleRequest request, Guid currentUserId);
    Task<ArticleDto> UpdateAsync(Guid id, UpdateArticleRequest request);
    Task DeleteAsync(Guid id);

    Task<ArticleDto> GetByIdAsync(Guid id);
    Task<IEnumerable<ArticleDto>> GetAllAsync();

    Task<List<ArticleDto>> GetRelatedByCategoryAsync(Guid categoryId, Guid currentArticleId);

    Task<PagedResult<ArticleDto>> GetArticlesAsync(
        string? search,
        Guid? categoryId,
        Guid? tagId,
        int page,
        int pageSize);

    Task<List<CategoryDto>> GetCategoriesAsync();

    Task<List<TagDto>> GetTagsByCategoryAsync(Guid? categoryId);

    Task<ArticleDto> GetArticleBySlugAsync(string slug);

    Task<List<ArticleDto>> GetLatestArticlesAsync(int count);
}