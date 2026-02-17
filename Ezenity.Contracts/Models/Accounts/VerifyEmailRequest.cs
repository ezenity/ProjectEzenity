using System.ComponentModel.DataAnnotations;

namespace Ezenity.Contracts.Models.Accounts;

/// <summary>
/// Represents the request payload for verifying an email.
/// </summary>
public class VerifyEmailRequest
{
    /// <summary>
    /// Gets or sets the verification token.
    /// </summary>
    [Required]
    public string Token { get; set; }
}
