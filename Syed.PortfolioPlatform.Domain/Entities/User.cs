using Syed.PortfolioPlatform.Domain.Enums;

namespace Syed.PortfolioPlatform.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;

        public string PasswordHash { get; set; } = default!;

        public UserRoleEnum Role { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; }
            = new List<RefreshToken>();
    }
}
