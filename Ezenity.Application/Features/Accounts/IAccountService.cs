using Ezenity.Contracts;
using Ezenity.Contracts.Models.Accounts;
using Ezenity.Domain.Entities.Accounts;

namespace Ezenity.Application.Features.Accounts;

/// <summary>
/// Defines operations related to account management.
/// </summary>
public interface IAccountService : IBaseService<Account, AccountResponse, CreateAccountRequest, UpdateAccountRequest, DeleteResponse>
{
    /// <summary>
    /// Authenticates the user based on the provided model.
    /// </summary>
    Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model, string v);

    /// <summary>
    /// Refreshes the authentication token.
    /// </summary>
    Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress);

    /// <summary>
    /// Revokes the provided token.
    /// </summary>
    Task RevokeTokenAsync(string token, string ipAddress);

    /// <summary>
    /// Registers a new user.
    /// </summary>
    Task RegisterAsync(RegisterRequest model, string origin);

    /// <summary>
    /// Verifies the email of the user with the given token.
    /// </summary>
    Task VerifyEmailAsync(string token);

    /// <summary>
    /// Sends a password reset email.
    /// </summary>
    Task ForgotPasswordAsync(ForgotPasswordRequest model, string origin);

    /// <summary>
    /// Validates the reset token.
    /// </summary>
    Task ValidateResetTokenAsync(ValidateResetTokenRequest model);

    /// <summary>
    /// Resets the user's password.
    /// </summary>
    Task ResetPasswordAsync(ResetPasswordRequest model);
}
