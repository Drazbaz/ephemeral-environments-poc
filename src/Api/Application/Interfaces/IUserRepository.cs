using Api.Application.Commands;
using Api.Application.Models;
using Api.Domain.Entities;

namespace Api.Application.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<User?> GetUserAsync(Guid userId);
        Task<PagedResult<User>> GetUsersAsync(GetUsersCommand command);
    }
}
