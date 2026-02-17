using Ezenity.Domain.Entities.Accounts;

namespace Ezenity.Application.Abstractions.Security;
public interface ITokenService
{
    string GenerateJwtToken(int accountId);
    RefreshToken GenerateNewRefreshToken(string ipAddress);
    void UpdateRefreshToken(RefreshToken refreshToken, RefreshToken newRefreshToken, string ipAddress);
    void RemoveOldRefreshTokens(Account account);
    Task<(RefreshToken, Account)> GetRefreshTokenAsync(string token);
    string RandomTokenString();
}
