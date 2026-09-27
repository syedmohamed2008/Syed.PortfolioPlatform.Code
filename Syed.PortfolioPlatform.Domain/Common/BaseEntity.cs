namespace Syed.PortfolioPlatform.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        // Add-Migration AddYouTubeLinkToArticle -Project Syed.PortfolioPlatform.Infrastructure -StartupProject Syed.PortfolioPlatform.API

        // Update-Database -Project Syed.PortfolioPlatform.Infrastructure -StartupProject Syed.PortfolioPlatform.API


    }
}
