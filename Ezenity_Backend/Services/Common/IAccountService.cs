using Ezenity_Backend.Entities.Accounts;
using Ezenity_Backend.Models;
using Ezenity_Backend.Models.Accounts;
using System.Threading.Tasks;

namespace Ezenity_Backend.Services.Common
{
    public interface IAccountService : IBaseService<Account, AccountResponse, CreateAccountRequest, UpdateAccountRequest, DeleteResponse>
    {
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model, string v);
        Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress);
        Task RevokeTokenAsync(string token, string ipAddress);
        Task RegisterAsync(RegisterRequest model, string origin);
        Task VerifyEmailAsync(string token);
        Task ForgotPasswordAsync(ForgotPasswordRequest model, string origin);
        Task ValidateResetTokenAsync(ValidateResetTokenRequest model);
        Task ResetPasswordAsync(ResetPasswordRequest model);
    }
}
