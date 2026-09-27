using Syed.PortfolioPlatform.Domain.Entities;

public interface ITagRepository
{
    Task<bool> IsDuplicateAsync(string name, string slug, Guid? excludeId = null);

    Task AddAsync(Tag tag);

    Task<Tag?> GetByIdForUpdateAsync(Guid id);

    Task<Tag?> GetForDeleteAsync(Guid id);

    Task<Tag?> GetByIdAsync(Guid id);

    Task<IEnumerable<Tag>> GetAllAsync();

    Task<List<Tag>> GetTagsByCategoryAsync(Guid categoryId);

    Task SaveChangesAsync();
}