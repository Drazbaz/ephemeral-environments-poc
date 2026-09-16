using Infrastructure.Data;

namespace Infrastructure
{
    public sealed class UnitOfWork(Context context) : IUnitOfWork
    {
        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return context.SaveChangesAsync(cancellationToken);
        }
    }
}
