using Api.Application.Handlers;
using Api.Application.Interfaces;
using Api.Infrastructure.Persistance;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Api.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionStringBuilder = new StringBuilder(configuration.GetConnectionString("Database"));
            var connectionString = connectionStringBuilder
                .Replace("DB_USER", configuration["DB_USER"])
                .Replace("DB_PASSWORD", configuration["DB_PASSWORD"])
                .ToString();
            services.AddDbContext<ApiDbContext>(options =>
                options.UseNpgsql(connectionString));
            services.AddScoped<GetUsersHandler>();
            services.AddScoped<CreateUserHandler>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
