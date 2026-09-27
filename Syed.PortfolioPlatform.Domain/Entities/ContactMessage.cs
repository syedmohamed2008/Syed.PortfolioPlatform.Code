namespace Syed.PortfolioPlatform.Domain.Entities
{
    public class ContactMessage
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string Message { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
