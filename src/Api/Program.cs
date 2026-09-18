using Api.Infrastructure;
using Api.Presentation.Endpoints;

namespace Api
{
    public partial class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddOpenApi();
            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseHttpsRedirection();

            var api = app.MapGroup("/api");
            UserEndpoints.Map(api);

            app.Run();
        }
    }
}