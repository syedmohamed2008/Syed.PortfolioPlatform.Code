using Syed.PortfolioPlatform.Domain.Entities;

public interface IContactRepository
{
    Task AddAsync(ContactMessage entity);
    Task SaveChangesAsync();
}