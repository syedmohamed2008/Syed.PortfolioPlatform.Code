using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Syed.PortfolioPlatform.Application.Common.Interface;
using Syed.PortfolioPlatform.Application.Interfaces;
using Syed.PortfolioPlatform.Infrastructure.AzureFunctions;
using Syed.PortfolioPlatform.Infrastructure.Persistence;
using Syed.PortfolioPlatform.Infrastructure.Persistence.Seed;
using Syed.PortfolioPlatform.Infrastructure.Repositories;
using Syed.PortfolioPlatform.Infrastructure.Storage;

namespace Syed.PortfolioPlatform.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                        sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(10),
                                errorNumbersToAdd: null);
                        }));

            // Repositories ONLY
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>(); 
            services.AddScoped<IAuthRepository, AuthRepository>();

            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<IImageUrlBuilder, ImageUrlBuilder>();

            services.AddScoped<IMessageBusService, ServiceBusService>();

            return services;
        }

        public static async Task<IApplicationBuilder> UseInfrastructureAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var dbContext = services.GetRequiredService<ApplicationDbContext>();

                Console.WriteLine("🚀 Starting DB Migration...");
                await dbContext.Database.MigrateAsync();
                Console.WriteLine("✅ DB Migration completed");

                await DataSeeder.SeedAsync(app.ApplicationServices);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Migration failed: " + ex.Message);
                throw;  
            }

            return app;
        }
    }
}