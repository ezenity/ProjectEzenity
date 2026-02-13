using Ezenity.Application.Abstractions.Persistence;
using Ezenity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ezenity.Infrastructure.Persistence;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly DataContext _db;

    public EfUnitOfWork(DataContext db) => _db = db;

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public async Task<IAppTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        var tx = await _db.Database.BeginTransactionAsync(ct);
        return new EfTransaction(tx);
    }
}
