using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Persistance
{
    public sealed class ApiDbContext(DbContextOptions<ApiDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
                .HasData(
                    new User
                    {
                        Id = Guid.NewGuid(),
                        Name = "Drazen Gordon" 
                    }
                );
        }
    }
}
