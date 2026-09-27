using Syed.PortfolioPlatform.Application.Common;
using Syed.PortfolioPlatform.Application.DTOs.Articles;
using Syed.PortfolioPlatform.Application.DTOs.Categories;
using Syed.PortfolioPlatform.Application.DTOs.Tags;
using Syed.PortfolioPlatform.Domain.Entities;

public interface IArticleRepository
{
    Task AddAsync(Article article);

    Task<Article?> GetByIdForUpdateAsync(Guid id);

    Task<ArticleDto?> GetByIdAsync(Guid id);

    Task SaveChangesAsync();

    Task<Article?> GetForDeleteAsync(Guid id);

    Task<IEnumerable<ArticleDto>> GetAllAsync();

    Task<List<ArticleDto>> GetRelatedByCategoryAsync(Guid categoryId, Guid currentArticleId);

    Task<(List<ArticleDto> Items, int Total)> GetPagedAsync(
        string? search,
        Guid? categoryId,
        Guid? tagId,
        int page,
        int pageSize);

    Task<ArticleDto?> GetBySlugAsync(string slug);

    Task<List<ArticleDto>> GetLatestAsync(int count);

    Task<List<CategoryDto>> GetCategoriesAsync();

    Task<List<TagDto>> GetTagsByCategoryAsync(Guid categoryId);
}