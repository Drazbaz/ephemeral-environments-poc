using Api.Application.Interfaces;
using Api.Application.Models;
using Api.Domain.Entities;
using Api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories
{
    public sealed class UserRepository(ApiDbContext dbContext) : IUserRepository
    {
        public async Task AddUserAsync(User user)
        {
            await dbContext.Users.AddAsync(user);
        }

        public async Task<PagedResult<User>> GetUsersAsync(Pagination pagination)
        {
            var users = await dbContext
                .Users
                .Skip(pagination.Offset)
                .Take(pagination.QueryLimit)
                .ToListAsync();

            var result = new PagedResult<User>
            {
                Items = [.. users.Take(pagination.Limit)],
                HasMore = users.Count > pagination.Limit
            };

            return result;
        }
    }
}
