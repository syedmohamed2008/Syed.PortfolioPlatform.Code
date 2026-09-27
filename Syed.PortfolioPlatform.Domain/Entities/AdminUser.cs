using Syed.PortfolioPlatform.Domain.Common;

namespace Syed.PortfolioPlatform.Domain.Entities
{
    public class AdminUser : BaseEntity
    {
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        // Role
        public Guid RoleId { get; set; }
        public Role RoleNavigation { get; set; } = null!;

        // Refresh Tokens
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        // Optional extra
        public ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}
