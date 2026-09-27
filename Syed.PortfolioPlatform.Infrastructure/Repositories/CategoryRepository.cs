using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syed.PortfolioPlatform.Infrastructure.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Syed.PortfolioPlatform.Application.Common.Interface;
    using Syed.PortfolioPlatform.Domain.Entities;
    using Syed.PortfolioPlatform.Infrastructure.Persistence;

    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsDuplicateAsync(string name, string slug, Guid? excludeId = null)
        {
            return await _context.Categories
                .AnyAsync(x =>
                    (x.Name == name || x.Slug == slug) &&
                    (!excludeId.HasValue || x.Id != excludeId));
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories
                .AsNoTracking()
                .Include(c => c.Articles)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<Category?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .Include(c => c.Articles)
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
