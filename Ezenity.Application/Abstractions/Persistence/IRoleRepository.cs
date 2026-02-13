using Ezenity.Core.Entities.Accounts;

namespace Ezenity.Application.Abstractions.Persistence;

public interface IRoleRepository
{
    Task<int> CountAccountsAsync(CancellationToken ct = default);
    Task<Role?> GetByNameAsync(string name, CancellationToken ct = default);
    void Add(Role role);
}
