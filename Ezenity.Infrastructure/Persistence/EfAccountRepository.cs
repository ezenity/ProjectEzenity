using Ezenity.Application.Abstractions.Persistence;
using Ezenity.Core.Entities.Accounts;
using Ezenity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ezenity.Infrastructure.Persistence;

public sealed class EfAccountRepository : IAccountRepository
{
    private readonly DataContext _db;

    public EfAccountRepository(DataContext db) => _db = db;

    public IQueryable<Account> Query()
        => _db.Accounts.AsQueryable();

    public Task<Account?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Accounts.FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task<Account?> GetByEmailAsync(string email, CancellationToken ct = default)
        => _db.Accounts.FirstOrDefaultAsync(a => a.Email == email, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        => _db.Accounts.AnyAsync(a => a.Email == email, ct);

    public void Add(Account account) => _db.Accounts.Add(account);
    public void Update(Account account) => _db.Accounts.Update(account);
    public void Remove(Account account) => _db.Accounts.Remove(account);
}
