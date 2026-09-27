using Syed.PortfolioPlatform.Domain.Entities;
using Syed.PortfolioPlatform.Infrastructure.Persistence;

public class ContactRepository : IContactRepository
{
    private readonly ApplicationDbContext _context;

    public ContactRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ContactMessage entity)
    {
        await _context.ContactMessages.AddAsync(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}