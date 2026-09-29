using Api.Application.Commands;
using Api.Application.Interfaces;
using Api.Application.Responses;
using Api.Domain.Entities;

namespace Api.Application.Handlers
{
    public sealed class CreateUserHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        public async Task<IResult> HandleAsync(CreateUserCommand command)
        {
            var user = new User
            {
                Name = command.Name
            };

            await userRepository.AddUserAsync(user);
            await unitOfWork.CommitAsync();

            var data = UserResponse.FromUser(user);
            return TypedResults.Created($"/api/user/{data.Id}", data);
        }
    }
}
