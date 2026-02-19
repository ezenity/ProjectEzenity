using Ezenity.Domain.Entities.Accounts;

namespace Ezenity.Application.Abstractions.Persistence;

public interface IAccountRepository
{
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<Account?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<Account?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
    Task<Account?> GetByVerificationTokenAsync(string token, CancellationToken ct = default);
    /**
     * Reset token validity check belongs here
     */
    Task<Account?> GetValidResetTokenAccountAsync(string token, CancellationToken ct = default);

    /**
     * For paging/search (still EF-backed in Infrastructure)
     */
    IQueryable<Account> Query();

    void Add(Account account);
    void Update(Account account);
    void Remove(Account account);
}
