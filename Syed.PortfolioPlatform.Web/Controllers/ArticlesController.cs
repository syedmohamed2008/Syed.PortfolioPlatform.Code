using Microsoft.AspNetCore.Mvc;
using Syed.PortfolioPlatform.Application.Common;
using Syed.PortfolioPlatform.Application.DTOs.Articles;
using Syed.PortfolioPlatform.Application.DTOs.Categories;
using Syed.PortfolioPlatform.Application.DTOs.Tags;
using Syed.PortfolioPlatform.Application.Interfaces;
using Syed.PortfolioPlatform.Web.Configurations;
using Syed.PortfolioPlatform.Web.Models;
using Syed.PortfolioPlatform.Web.ViewModels.Articles;
using System.Text.RegularExpressions;

namespace Syed.PortfolioPlatform.Web.Controllers
{
    public class ArticlesController : BaseController
    {
        private readonly HttpClient _httpClient;
        private readonly IImageUrlBuilder _imageUrlBuilder;

        private static readonly Regex HtmlRegex =
            new Regex("<.*?>", RegexOptions.Compiled);

        public ArticlesController(IHttpClientFactory httpClientFactory, PortfolioSettings settings, IImageUrlBuilder imageUrlBuilder)
            : base(settings)
        {
            _httpClient = httpClientFactory.CreateClient("api");
            _imageUrlBuilder = imageUrlBuilder;
        }

        public async Task<IActionResult> Index(string search, Guid? categoryId, Guid? tagId, int page = 1)
        {
            int pageSize = 6;

            var articlesTask = await _httpClient.GetFromJsonAsync<PagedResult<ArticleDto>>(
                $"api/articles?search={search}&categoryId={categoryId}&tagId={tagId}&page={page}&pageSize={pageSize}");

            var categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>(
                "api/articles/categories");

            var tags = await _httpClient.GetFromJsonAsync<List<TagDto>>(
                $"api/articles/tags?categoryId={categoryId}");

            

            var viewModel = new ArticleListViewModel
            {
                Articles = articlesTask?.Items ?? new List<ArticleDto>(),
                Categories = categories ?? new List<CategoryDto>(),
                Tags = tags ?? new List<TagDto>(),
                Search = search,
                CategoryId = categoryId,
                TagId = tagId,
                CurrentPage = page,
                TotalPages = articlesTask?.TotalPages ?? 0
            };

            foreach (var article in viewModel.Articles)
            {
                article.ImageUrl = _imageUrlBuilder.Build(article.ImageUrl);
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMore(string search, Guid? categoryId, Guid? tagId, int page = 1)
        {
            int pageSize = 6;

            var result = await _httpClient.GetFromJsonAsync<PagedResult<ArticleDto>>(
                $"api/articles?search={search}&categoryId={categoryId}&tagId={tagId}&page={page}&pageSize={pageSize}");

            if (result?.Items == null || !result.Items.Any())
                return Content("");

            foreach (var article in result?.Items ?? new List<ArticleDto>())
            {
                article.ImageUrl = _imageUrlBuilder.Build(article.ImageUrl);
            }

            return PartialView("_ArticleRow", result.Items);
        }



        //[HttpGet("get-tags-by-category")]
        public async Task<IActionResult> GetTagsByCategory(Guid? categoryId)
        {
            try
            {
                var tags = await _httpClient.GetFromJsonAsync<List<TagDto>>(
                    $"api/articles/tags?categoryId={categoryId}");

                return Ok(tags ?? new List<TagDto>());
            }
            catch
            {
                return Ok(new List<TagDto>());
            }
        }

        [HttpGet("view/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            var article = await _httpClient.GetFromJsonAsync<ArticleDto>(
                $"api/articles/slug/{slug}");

            if (article == null)
                return NotFound();

            var plainText = HtmlRegex.Replace(article.Content ?? "", "");

            var wordCount = plainText
                .Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Length;

            var readingTime = Math.Ceiling(wordCount / 200.0);

            var relatedArticles = await _httpClient.GetFromJsonAsync<List<ArticleDto>>(
                $"api/articles/related?categoryId={article.CategoryId}&articleId={article.Id}");

            var viewModel = new ArticleDetailsViewModel
            {
                Article = article,
                Tags = article.Tags,
                RelatedArticles = relatedArticles ?? new List<ArticleDto>(),
                ReadingTime = readingTime
            };

            return View(viewModel);
        }
    }
}