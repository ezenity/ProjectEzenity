using Ezenity.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ezenity.Infrastructure.Persistence;

public sealed class EfTransaction : IAppTransaction
{
    private readonly IDbContextTransaction _tx;

    public EfTransaction(IDbContextTransaction tx) => _tx = tx;

    public Task CommitAsync(CancellationToken ct = default) => _tx.CommitAsync(ct);
    public Task RollbackAsync(CancellationToken ct = default) => _tx.RollbackAsync(ct);

    public ValueTask DisposeAsync() => _tx.DisposeAsync();
}
