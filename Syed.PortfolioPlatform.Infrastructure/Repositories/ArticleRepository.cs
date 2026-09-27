using Microsoft.EntityFrameworkCore;
using Syed.PortfolioPlatform.Application.DTOs.Articles;
using Syed.PortfolioPlatform.Application.DTOs.Categories;
using Syed.PortfolioPlatform.Application.DTOs.Tags;
using Syed.PortfolioPlatform.Domain.Entities;
using Syed.PortfolioPlatform.Infrastructure.Persistence;
using System.Text.RegularExpressions;

public class ArticleRepository : IArticleRepository
{
    private readonly ApplicationDbContext _context;

    private static readonly Regex HtmlRegex =
    new("<[^>]*>", RegexOptions.Compiled);

    public ArticleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Article article)
    {
        await _context.Articles.AddAsync(article);
    }

    public async Task<Article?> GetByIdForUpdateAsync(Guid id)
    {
        return await _context.Articles
            .Include(a => a.ArticleTags)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
    }

    public async Task<Article?> GetForDeleteAsync(Guid id)
    {
        return await _context.Articles
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
    }

    public async Task<ArticleDto?> GetByIdAsync(Guid id)
    {
        return await _context.Articles
            .AsNoTracking()
            .Where(a => a.Id == id && !a.IsDeleted)
            .Select(a => new ArticleDto
            {
                Id = a.Id,
                Title = a.Title,
                Slug = a.Slug,
                Content = a.Content,
                ImageUrl = a.ImageUrl,
                CategoryId = a.CategoryId,
                CategoryName = a.Category.Name,
                PublishedDate = a.PublishedDate,
                IsPublished = a.IsPublished,
                YouTubeLink = a.YouTubeLink,
                PDFLink = a.PDFLink,
                Views = a.Views,
                CreatedOn = a.CreatedOn,
                CreatedByUserId = a.CreatedByUserId,
                CreatedByUsername = a.CreatedByUser.Username,
                Tags = a.ArticleTags.Select(t => new TagDto
                {
                    Id = t.Tag.Id,
                    Name = t.Tag.Name
                }).ToList(),
                TagIds = a.ArticleTags.Select(t => t.TagId).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ArticleDto>> GetAllAsync()
    {
        return await _context.Articles
            .AsNoTracking()
            .Where(a => !a.IsDeleted)
            .OrderByDescending(a => a.CreatedOn)
            .Select(a => new ArticleDto
            {
                Id = a.Id,
                Title = a.Title,
                Slug = a.Slug,
                ImageUrl = a.ImageUrl,
                CategoryName = a.Category.Name,
                PublishedDate = a.CreatedOn,
                Views = a.Views
            })
            .ToListAsync();
    }

    public async Task<List<ArticleDto>> GetRelatedByCategoryAsync(Guid categoryId, Guid currentArticleId)
    {
        return await _context.Articles
            .AsNoTracking()
            .Where(a => a.CategoryId == categoryId &&
                        a.Id != currentArticleId &&
                        !a.IsDeleted &&
                        a.IsPublished)
            .OrderByDescending(a => a.CreatedOn)
            .Take(5)
            .Select(a => new ArticleDto
            {
                Id = a.Id,
                Title = a.Title,
                Slug = a.Slug,
                ImageUrl = a.ImageUrl
            })
            .ToListAsync();
    }

    public async Task<(List<ArticleDto> Items, int Total)> GetPagedAsync(
        string? search,
        Guid? categoryId,
        Guid? tagId,
        int page,
        int pageSize)
    {
        var query = _context.Articles
            .AsNoTracking()
            .Where(a => !a.IsDeleted && a.IsPublished);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(a => a.Title.Contains(search));

        if (categoryId.HasValue)
            query = query.Where(a => a.CategoryId == categoryId);

        if (tagId.HasValue)
            query = query.Where(a => a.ArticleTags.Any(t => t.TagId == tagId));

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new ArticleDto
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                Slug = a.Slug,
                ImageUrl = a.ImageUrl,
                YouTubeLink = a.YouTubeLink,
                PDFLink = a.PDFLink,
                Tags = a.ArticleTags
    .Take(1)
    .Select(at => new TagDto
    {
        Id = at.Tag.Id,
        Name = at.Tag.Name
    })
    .ToList(),
                CategoryName = a.Category.Name,
                PublishedDate = a.PublishedDate,
                CreatedOn = a.CreatedOn,
                Views = a.Views
            })
            .ToListAsync();

        for (int i = 0; i < items.Count; i++)
        {
            var plainText = HtmlRegex.Replace(items[i].Content ?? "", "");

            var wordCount = plainText
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
                .Length;

            items[i].ReadingTime = Math.Max(
                1,
                (int)Math.Ceiling(wordCount / 200.0)
            );

            items[i].Content = plainText.Length > 250
                ? plainText[..250]
                : plainText;
        }


        return (items, total);
    }

    public async Task<ArticleDto?> GetBySlugAsync(string slug)
    {
        return await _context.Articles
            .AsNoTracking()
            .Where(a => a.Slug == slug && !a.IsDeleted)
            .Select(a => new ArticleDto
            {
                Id = a.Id,
                Title = a.Title,
                Slug = a.Slug,
                Content = a.Content,
                ImageUrl = a.ImageUrl,
                YouTubeLink= a.YouTubeLink,
                PDFLink = a.PDFLink,
                CategoryName = a.Category.Name,
                CreatedByUsername = a.CreatedByUser.Username,
                PublishedDate = a.PublishedDate,
                CreatedOn = a.CreatedOn,
                Views = a.Views,
                Tags = a.ArticleTags.Select(t => new TagDto
                {
                    Id = t.Tag.Id,
                    Name = t.Tag.Name
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<ArticleDto>> GetLatestAsync(int count)
    {
        return await _context.Articles
            .AsNoTracking()
            .Where(a => !a.IsDeleted && a.IsPublished)
            .OrderByDescending(a => a.CreatedOn)
            .Take(count)
            .Select(a => new ArticleDto
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content.Substring(0,250),
                Slug = a.Slug,
                ImageUrl = a.ImageUrl,
                CategoryName = a.Category.Name,
                PublishedDate = a.PublishedDate,
                CreatedOn = a.CreatedOn
            })
            .ToListAsync();
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug
            })
            .ToListAsync();
    }

    public async Task<List<TagDto>> GetTagsByCategoryAsync(Guid categoryId)
    {
        return await _context.Tags
            .Where(t => t.ArticleTags.Any(at =>
                   at.Article.CategoryId == categoryId &&
                   at.Article.IsPublished &&
                   !at.Article.IsDeleted))
           .Select(t => new TagDto
           {
               Id = t.Id,
               Name = t.Name
           })
           .Distinct()
           .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}