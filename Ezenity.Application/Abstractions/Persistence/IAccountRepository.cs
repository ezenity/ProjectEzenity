using Ezenity.Domain.Entities.Accounts;
using System.Security.Principal;

namespace Ezenity.Application.Abstractions.Persistence;

public interface IAccountRepository
{
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<Account?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<Account?> GetByIdAsync(int id, CancellationToken ct = default);

    IQueryable<Account> Query(); // for paging/search (still EF-backed in Infrastructure)

    void Add(Account account);
    void Update(Account account);
    void Remove(Account account);
}
