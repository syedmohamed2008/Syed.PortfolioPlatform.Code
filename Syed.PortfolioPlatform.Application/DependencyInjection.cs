using Microsoft.Extensions.DependencyInjection;
using Syed.PortfolioPlatform.Application.Interfaces;
using Syed.PortfolioPlatform.Infrastructure.Services;

namespace Syed.PortfolioPlatform.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Services ONLY (business logic)
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}