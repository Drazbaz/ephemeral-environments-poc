using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Persistance
{
    public static class DbContextExtensions
    {
        public static IHost MigrateDatabase<TDbContext>(this IHost host)
            where TDbContext : DbContext
        {
            using var scope = host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            dbContext.Database.Migrate();

            return host;
        }
    }
}
