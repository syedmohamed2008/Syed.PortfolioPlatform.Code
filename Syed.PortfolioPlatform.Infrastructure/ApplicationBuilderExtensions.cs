using Microsoft.AspNetCore.Builder;
using Syed.PortfolioPlatform.Infrastructure.Persistence.Seed;

namespace Syed.PortfolioPlatform.Infrastructure.Persistence
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task<IApplicationBuilder> UseInfrastructureAsync(
            this IApplicationBuilder app)
        {
            await DataSeeder.SeedAsync(app.ApplicationServices);
            return app;
        }
    }
}