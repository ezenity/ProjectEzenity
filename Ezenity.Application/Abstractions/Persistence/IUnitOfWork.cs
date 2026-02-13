namespace Ezenity.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task<IAppTransaction> BeginTransactionAsync(CancellationToken ct = default);
}
