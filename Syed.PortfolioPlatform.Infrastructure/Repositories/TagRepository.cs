using Microsoft.EntityFrameworkCore;
using Syed.PortfolioPlatform.Domain.Entities;
using Syed.PortfolioPlatform.Infrastructure.Persistence;

public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _context;

    public TagRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsDuplicateAsync(string name, string slug, Guid? excludeId = null)
    {
        return await _context.Tags
            .AnyAsync(x =>
                (x.Name == name || x.Slug == slug) &&
                (!excludeId.HasValue || x.Id != excludeId));
    }

    public async Task AddAsync(Tag tag)
    {
        await _context.Tags.AddAsync(tag);
    }

    public async Task<Tag?> GetByIdForUpdateAsync(Guid id)
    {
        return await _context.Tags
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<Tag?> GetForDeleteAsync(Guid id)
    {
        return await _context.Tags
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
    }

    public async Task<Tag?> GetByIdAsync(Guid id)
    {
        return await _context.Tags
            .AsNoTracking()
            .Include(t => t.ArticleTags)
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        return await _context.Tags
            .AsNoTracking()
            .Include(t => t.ArticleTags)
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<List<Tag>> GetTagsByCategoryAsync(Guid categoryId)
    {
        return await _context.Tags
            .AsNoTracking()
            .Where(t => t.ArticleTags.Any(at =>
                at.Article.CategoryId == categoryId &&
                at.Article.IsPublished &&
                !at.Article.IsDeleted))
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}