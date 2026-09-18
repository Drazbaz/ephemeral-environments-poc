using Api.Application.Interfaces;

namespace Api.Infrastructure.Persistance
{
    public sealed class UnitOfWork(ApiDbContext dbContext) : IUnitOfWork
    {
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
