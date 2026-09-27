namespace Syed.PortfolioPlatform.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public string Token { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }

        // Link to AdminUser
        public Guid AdminId { get; set; }
        public AdminUser Admin { get; set; } = null!;
    }
}
