using Ezenity_Backend.Entities.Common;
using Ezenity_Backend.Models.Common.Accounts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ezenity_Backend.Services.Common
{
    public interface IAccountService : IBaseService<IAccount, IAccountResponse, ICreateAccountRequest, IUpdateAccountRequest>
    {
        Task<IAuthenticateResponse> AuthenticateAsync(IAuthenticateRequest model, string v);
        Task<IAuthenticateResponse> RefreshTokenAsync(string token, string ipAddress);
        Task RevokeTokenAsync(string token, string ipAddress);
        Task RegisterAsync(IRegisterRequest model, string origin);
        Task VerifyEmailAsync(string token);
        Task ForgotPasswordAsync(IForgotPasswordRequest model, string origin);
        Task ValidateResetTokenAsync(IValidateResetTokenRequest model);
        Task ResetPasswordAsync(IResetPasswordRequest model);
        Task<IEnumerable<IAccountResponse>> GetAllAsync();
        Task<IAccountResponse> GetByIdAsync(int id);
        Task<IAccountResponse> CreateAsync(ICreateAccountRequest model);
        Task<IAccountResponse> UpdateAsync(int id, IUpdateAccountRequest model);
        Task DeleteAsync(int id);
    }
}
