using Api.Application.Commands;
using Api.Application.Interfaces;
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

            return TypedResults.Created();
        }
    }
}
