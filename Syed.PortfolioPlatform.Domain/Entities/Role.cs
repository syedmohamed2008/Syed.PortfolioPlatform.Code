using Syed.PortfolioPlatform.Domain.Common;

namespace Syed.PortfolioPlatform.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = null!;
        public ICollection<AdminUser> AdminUsers { get; set; } = new List<AdminUser>();
    }
}
