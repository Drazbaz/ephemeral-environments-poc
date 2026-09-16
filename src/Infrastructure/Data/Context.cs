using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public sealed class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        public DbSet<Person> People { get; set; } = null!;
    }
}
