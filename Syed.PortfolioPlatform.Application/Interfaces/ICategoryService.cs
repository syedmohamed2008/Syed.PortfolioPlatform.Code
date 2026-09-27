using Syed.PortfolioPlatform.Application.DTOs.Categories;

namespace Syed.PortfolioPlatform.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateAsync(CreateCategoryRequest request);
        Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryRequest request);
        Task DeleteAsync(Guid id);
        Task<CategoryDto> GetByIdAsync(Guid id);
        Task<IEnumerable<CategoryDto>> GetAllAsync();
    }
}
