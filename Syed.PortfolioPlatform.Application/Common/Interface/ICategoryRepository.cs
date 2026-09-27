using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syed.PortfolioPlatform.Application.Common.Interface
{
    using Syed.PortfolioPlatform.Domain.Entities;

    public interface ICategoryRepository
    {
        Task<bool> IsDuplicateAsync(string name, string slug, Guid? excludeId = null);

        Task AddAsync(Category category);

        Task<Category?> GetByIdAsync(Guid id);

        Task<Category?> GetByIdForUpdateAsync(Guid id);

        Task<IEnumerable<Category>> GetAllAsync();

        Task SaveChangesAsync();
    }
}
