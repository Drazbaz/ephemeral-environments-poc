using Api.Application.Models;
using Api.Domain.Entities;

namespace Api.Application.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<PagedResult<User>> GetUsersAsync(Pagination pagination);
    }
}
