using Domain.Entities;

namespace Infrastructure.Repositories
{
    public interface IPersonRepository
    {
        Task CreateAsync(Person person);
        Task<List<Person>> GetAllAsync();
    }
}
