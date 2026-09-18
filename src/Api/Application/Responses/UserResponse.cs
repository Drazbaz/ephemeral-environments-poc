using Api.Domain.Entities;

namespace Api.Application.Dtos
{
    public sealed record UserResponse(Guid Id, string Name)
    {
        public static UserResponse FromUser(User user)
        {
            return new UserResponse(user.Id, user.Name);
        }
    }
}
