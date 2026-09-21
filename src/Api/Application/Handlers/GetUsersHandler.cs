using Api.Application.Commands;
using Api.Application.Dtos;
using Api.Application.Interfaces;
using Api.Application.Models;

namespace Api.Application.Handlers
{
    public sealed class GetUsersHandler(IUserRepository userRepository)
    {
        public async Task<IResult> HandleAsync(GetUsersCommand command)
        {
            var users = await userRepository.GetUsersAsync(command);

            var data = new PagedResult<UserResponse>
            {
                Items = [.. users.Items.Select(UserResponse.FromUser)],
                HasMore = users.HasMore
            };

            return TypedResults.Ok(data);
        }
    }
}
