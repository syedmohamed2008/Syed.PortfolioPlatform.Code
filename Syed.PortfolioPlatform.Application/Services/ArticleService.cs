using Microsoft.Extensions.Configuration;
using Syed.PortfolioPlatform.Application.Common;
using Syed.PortfolioPlatform.Application.DTOs.Articles;
using Syed.PortfolioPlatform.Application.DTOs.Categories;
using Syed.PortfolioPlatform.Application.DTOs.Tags;
using Syed.PortfolioPlatform.Application.Interfaces;
using Syed.PortfolioPlatform.Domain.Entities;
using System.Text.Json;

namespace Syed.PortfolioPlatform.Infrastructure.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly IImageUrlBuilder _imageUrlBuilder;
        private readonly IMessageBusService _messageBusService;

        public ArticleService(IArticleRepository repository, IConfiguration configuration, IImageUrlBuilder imageUrlBuilder, IMessageBusService messageBusService)
        {
            _repository = repository;
            _configuration = configuration;
            _imageUrlBuilder = imageUrlBuilder;
            _messageBusService = messageBusService;
        }

        // ----------------- CREATE -----------------
        public async Task<ArticleDto> CreateAsync(CreateArticleRequest request, Guid currentUserId)
        {
            var article = new Article
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Slug = request.Slug,
                Content = request.Content,
                ImageUrl = request.ImageUrl,
                YouTubeLink = request.YouTubeLink,
                PDFLink = request.PDFLink,
                CategoryId = request.CategoryId,
                MetaTitle = request.MetaTitle,
                MetaDescription = request.MetaDescription,
                PublishedDate = request.PublishedDate,
                IsPublished = request.IsPublished,
                IsDeleted = false,
                Views = 0,
                CreatedByUserId = currentUserId
            };

            if (request.TagIds != null && request.TagIds.Any())
            {
                foreach (var tagId in request.TagIds)
                {
                    article.ArticleTags.Add(new ArticleTag
                    {
                        TagId = tagId,
                        ArticleId = article.Id
                    });
                }
            }

            await _repository.AddAsync(article);
            await _repository.SaveChangesAsync();

            if (article.IsPublished)
            {
                var payload = new
                {
                    Id = article.Id,
                    Title = article.Title,
                    Slug = article.Slug,
                    PublishedDate = article.PublishedDate ?? DateTime.UtcNow
                };

                await _messageBusService.PublishArticleAsync(JsonSerializer.Serialize(payload));
            }
            return await GetByIdAsync(article.Id);
        }

        // ----------------- UPDATE -----------------
        public async Task<ArticleDto> UpdateAsync(Guid id, UpdateArticleRequest request)
        {
            var article = await _repository.GetByIdForUpdateAsync(id);

            if (article == null)
                throw new KeyNotFoundException("Article not found");

            article.Title = request.Title;
            article.Slug = request.Slug;
            article.Content = request.Content;

            if (!string.IsNullOrEmpty(request.ImageUrl))
                article.ImageUrl = request.ImageUrl;

            if (!string.IsNullOrEmpty(request.YouTubeLink))
                article.YouTubeLink = request.YouTubeLink;

            if (!string.IsNullOrEmpty(request.PDFLink))
                article.PDFLink = request.PDFLink;

            article.CategoryId = request.CategoryId;
            article.MetaTitle = request.MetaTitle;
            article.MetaDescription = request.MetaDescription;
            article.PublishedDate = request.PublishedDate;
            article.IsPublished = request.IsPublished;

            if (request.TagIds != null)
            {
                article.ArticleTags.Clear();

                foreach (var tagId in request.TagIds)
                {
                    article.ArticleTags.Add(new ArticleTag
                    {
                        TagId = tagId,
                        ArticleId = article.Id
                    });
                }
            }

            await _repository.SaveChangesAsync();

            return await GetByIdAsync(article.Id);
        }

        // ----------------- DELETE -----------------
        public async Task DeleteAsync(Guid id)
        {
            var article = await _repository.GetForDeleteAsync(id);

            if (article == null)
                throw new KeyNotFoundException("Article not found");

            article.IsDeleted = true;

            await _repository.SaveChangesAsync();
        }

        // ----------------- GET BY ID -----------------
        public async Task<ArticleDto> GetByIdAsync(Guid id)
        {
            var article = await _repository.GetByIdAsync(id);

            if (article == null)
                throw new KeyNotFoundException("Article not found");

            article.ImageUrl = _imageUrlBuilder.Build(article.ImageUrl);

            return article;
        }

        // ----------------- GET ALL -----------------
        public async Task<IEnumerable<ArticleDto>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        // ----------------- RELATED ARTICLES -----------------
        public async Task<List<ArticleDto>> GetRelatedByCategoryAsync(Guid categoryId, Guid currentArticleId)
        {
            return await _repository.GetRelatedByCategoryAsync(categoryId, currentArticleId);
        }

        // ----------------- PAGED ARTICLES -----------------
        public async Task<PagedResult<ArticleDto>> GetArticlesAsync(
            string? search,
            Guid? categoryId,
            Guid? tagId,
            int page,
            int pageSize)
        {

            var (items, total) = await _repository.GetPagedAsync(search, categoryId, tagId, page, pageSize);

          
            var results = new PagedResult<ArticleDto>
            {
                Items = items,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };

            foreach (var article in results.Items)
            {
                article.ImageUrl = _imageUrlBuilder.Build(article.ImageUrl);
            }
            return results;
        }




        // ----------------- GET BY SLUG -----------------
        public async Task<ArticleDto> GetArticleBySlugAsync(string slug)
        {
            var article = await _repository.GetBySlugAsync(slug);

            if (article == null)
                throw new KeyNotFoundException("Article not found");

            article.ImageUrl = _imageUrlBuilder.Build(article.ImageUrl);

            return article;
        }

        // ----------------- LATEST ARTICLES -----------------
        public async Task<List<ArticleDto>> GetLatestArticlesAsync(int count)
        {
            var articles = await _repository.GetLatestAsync(count);

            foreach (var article in articles)
            {
                article.ImageUrl = _imageUrlBuilder.Build(article.ImageUrl);
            }

            return articles;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            return await _repository.GetCategoriesAsync();
        }

        public async Task<List<TagDto>> GetTagsByCategoryAsync(Guid? categoryId)
        {
            return categoryId.HasValue
                    ? await _repository.GetTagsByCategoryAsync(categoryId.Value)
                    : new List<TagDto>();
        }
    }
}