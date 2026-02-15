using System.ComponentModel.DataAnnotations;

namespace Ezenity.Contracts.Models.Accounts;

/// <summary>
/// Represents the request payload for initiating a password reset operation.
/// </summary>
public class ForgotPasswordRequest
{
    /// <summary>
    /// Gets or sets the email of the account for which the password is forgotten.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; }

}
