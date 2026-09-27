using Syed.PortfolioPlatform.Application.DTOs.ContactMessage;
using Syed.PortfolioPlatform.Application.Interfaces;
using Syed.PortfolioPlatform.Domain.Entities;

namespace Syed.PortfolioPlatform.Infrastructure.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _repository;

        public ContactService(IContactRepository repository)
        {
            _repository = repository;
        }

        public async Task SaveMessageAsync(ContactMessageRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var entity = new ContactMessage
            {
                Name = request.Name,
                Email = request.Email,
                Message = request.Message,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
        }
    }
}