using Microsoft.EntityFrameworkCore;
using Syed.PortfolioPlatform.Domain.Common;
using Syed.PortfolioPlatform.Domain.Entities;

namespace Syed.PortfolioPlatform.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Article> Articles => Set<Article>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<ArticleTag> ArticleTags => Set<ArticleTag>();
        public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("dbo");

            // Apply Fluent API configs automatically
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedOn = DateTime.UtcNow;
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
