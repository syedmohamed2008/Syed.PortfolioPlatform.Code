using Microsoft.AspNetCore.Mvc;
using Syed.PortfolioPlatform.Application.DTOs.Tags;
using System.Threading.Tasks;

namespace Syed.PortfolioPlatform.Application.Interfaces
{
    public interface ITagService
    {
        Task<TagDto> CreateAsync(CreateTagRequest request);
        Task<TagDto> UpdateAsync(Guid id, UpdateTagRequest request);
        Task DeleteAsync(Guid id);
        Task<TagDto> GetByIdAsync(Guid id);
        Task<IEnumerable<TagDto>> GetAllAsync();

        Task<List<TagDto>> GetTagsByCategory(Guid categoryId);
    }
}
