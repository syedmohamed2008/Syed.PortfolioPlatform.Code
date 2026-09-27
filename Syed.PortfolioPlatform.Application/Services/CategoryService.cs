using Syed.PortfolioPlatform.Application.Common.Interface;
using Syed.PortfolioPlatform.Application.DTOs.Categories;
using Syed.PortfolioPlatform.Application.Interfaces;
using Syed.PortfolioPlatform.Domain.Entities;

namespace Syed.PortfolioPlatform.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<CategoryDto?> CreateAsync(CreateCategoryRequest request)
        {
            if (await _repository.IsDuplicateAsync(request.Name, request.Slug))
                return null;

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = request.Slug,
                Description = request.Description,
                IsDeleted = false
            };

            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<CategoryDto?> UpdateAsync(Guid id, UpdateCategoryRequest request)
        {
            var category = await _repository.GetByIdForUpdateAsync(id);

            if (category == null)
                return null;

            if (await _repository.IsDuplicateAsync(request.Name, request.Slug, id))
                return null;

            category.Name = request.Name;
            category.Slug = request.Slug;
            category.Description = request.Description;

            await _repository.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _repository.GetByIdForUpdateAsync(id);

            if (category == null)
                throw new KeyNotFoundException("Category not found");

            category.IsDeleted = true;

            await _repository.SaveChangesAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(Guid id)
        {
            var category = await _repository.GetByIdAsync(id);

            return category == null ? null : MapToDto(category);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();
            return categories.Select(MapToDto);
        }

        //private async Task<bool> IsDuplicateAsync(string name, string slug, Guid? excludeId = null)
        //{
        //    return await _repository.IsDuplicateAsync(name, slug, excludeId);
        //}

        private CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description,
                IsDeleted = category.IsDeleted,
                ArticleCount = category.Articles?.Count ?? 0
            };
        }
    }
}