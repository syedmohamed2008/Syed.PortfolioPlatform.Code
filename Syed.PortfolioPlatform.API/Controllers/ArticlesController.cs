using Microsoft.AspNetCore.Mvc;
using Syed.PortfolioPlatform.Application.Interfaces;

[ApiController]
[Route("api/articles")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetArticles(
      string? search,
      Guid? categoryId,
      Guid? tagId,
      int page = 1,
      int pageSize = 6)
    {
        var result = await _articleService.GetArticlesAsync(
            search, categoryId, tagId, page, pageSize);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var article = await _articleService.GetByIdAsync(id);
        return Ok(article);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var result = await _articleService.GetCategoriesAsync();
        return Ok(result);
    }

    [HttpGet("tags")]
    public async Task<IActionResult> GetTags(Guid? categoryId)
    {
        var result = await _articleService.GetTagsByCategoryAsync(categoryId);
        return Ok(result);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _articleService.GetArticleBySlugAsync(slug);
        return Ok(result);
    }

    [HttpGet("related")]
    public async Task<IActionResult> GetRelated(Guid categoryId, Guid articleId)
    {
        var result = await _articleService
            .GetRelatedByCategoryAsync(categoryId, articleId);

        return Ok(result);
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestArticles(int count = 3)
    {
        var articles = await _articleService.GetLatestArticlesAsync(count);

        return Ok(articles);
    }
}