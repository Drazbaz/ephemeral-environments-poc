using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class PersonRepository(Context context) : IPersonRepository
    {
        public async Task CreateAsync(Person person)
        {
            await context.People.AddAsync(person);
        }

        public async Task<List<Person>> GetAllAsync()
        {
            var people = await context.People.ToListAsync();
            return people;
        }
    }
}
