using Api.Application.Commands;
using Api.Application.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Api.Presentation.Endpoints
{
    public static class UserEndpoints
    {
        public static void Map(RouteGroupBuilder api)
        {
            var userApi = api.MapGroup("/user")
                .WithTags("User");

            userApi.MapGet("/", GetUsersAsync);
            userApi.MapPost("/", CreateUserAsync);
        }

        private static async Task<IResult> GetUsersAsync(
            [AsParameters] GetUsersCommand command,
            [FromServices] GetUsersHandler handler)
        {
            var result = await handler.HandleAsync(command);
            return result;
        }

        private static async Task<IResult> CreateUserAsync(
            [FromBody] CreateUserCommand command,
            [FromServices] CreateUserHandler handler)
        {
            var result = await handler.HandleAsync(command);
            return result;
        }
    }
}
