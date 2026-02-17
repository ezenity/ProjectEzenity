using Ezenity.Application.Abstractions.Persistence;
using Ezenity.Domain.Entities.Accounts;
using Ezenity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ezenity.Infrastructure.Persistence;

public sealed class EfRoleRepository : IRoleRepository
{
    private readonly DataContext _db;

    public EfRoleRepository(DataContext db) => _db = db;

    public Task<int> CountAccountsAsync(CancellationToken ct = default)
        => _db.Accounts.CountAsync(ct);

    public Task<Role?> GetByNameAsync(string name, CancellationToken ct = default)
        => _db.Roles.FirstOrDefaultAsync(r => r.Name == name, ct);

    public void Add(Role role) => _db.Roles.Add(role);
}
