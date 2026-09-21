using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Persistance
{
    public sealed class ApiDbContext(DbContextOptions<ApiDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<User>()
                .HasData(
                    new User
                    {
                        Id = Guid.Parse("61d3c5d7-5c15-4907-b196-7ff1d4a9e5af"),
                        Name = "Drazen Gordon" 
                    }
                );
        }
    }
}
