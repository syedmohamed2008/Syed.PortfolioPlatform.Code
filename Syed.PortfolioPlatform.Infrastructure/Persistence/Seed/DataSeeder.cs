using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Syed.PortfolioPlatform.Domain.Entities;
using Syed.PortfolioPlatform.Infrastructure.Persistence;

namespace Syed.PortfolioPlatform.Infrastructure.Persistence.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Auto migrate database
            await context.Database.MigrateAsync();

            bool changes = false;

            // ==============================
            // Seed Roles
            // ==============================
            if (!await context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = "Admin",
                        CreatedOn = DateTime.UtcNow
                    },
                    new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = "Editor",
                        CreatedOn = DateTime.UtcNow
                    }
                };

                await context.Roles.AddRangeAsync(roles);
                changes = true;
            }

            // ==============================
            // Seed Admin User
            // ==============================
            if (!await context.AdminUsers.AnyAsync())
            {
                var adminRole = await context.Roles
                    .FirstOrDefaultAsync(r => r.Name == "Admin");

                if (adminRole != null)
                {
                    var adminUser = new AdminUser
                    {
                        Id = Guid.NewGuid(),
                        Username = "admin",
                        RoleId = adminRole.Id,
                        CreatedOn = DateTime.UtcNow
                    };

                    var passwordHasher = new PasswordHasher<AdminUser>();
                    adminUser.PasswordHash =
                        passwordHasher.HashPassword(adminUser, "Admin@123");

                    await context.AdminUsers.AddAsync(adminUser);
                    changes = true;
                }
            }

            // ==============================
            // Seed Category
            // ==============================
            if (!await context.Categories.AnyAsync())
            {
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "General",
                    Slug = "general",
                    Description = "Default category",
                    CreatedOn = DateTime.UtcNow
                };

                await context.Categories.AddAsync(category);
                changes = true;
            }

            if (changes)
                await context.SaveChangesAsync();
        }
    }
}