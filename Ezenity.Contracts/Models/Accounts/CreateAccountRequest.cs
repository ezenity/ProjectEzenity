using System.ComponentModel.DataAnnotations;

namespace Ezenity.Contracts.Models.Accounts;

/// <summary>
/// Represents the request payload for creating a new account.
/// </summary>
public class CreateAccountRequest
{
    /// <summary>
    /// Gets or sets the title for the account holder.
    /// </summary>
    [Required]
    public required string Title { get; init; }

    /// <summary>
    /// Gets or sets the first name of the account holder.
    /// </summary>
    [Required]
    public required string FirstName { get; init; }

    /// <summary>
    /// Gets or sets the last name of the account holder.
    /// </summary>
    [Required]
    public required string LastName { get; init; }

    /// <summary>
    /// Gets or sets the email of the account holder.
    /// </summary>
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    /// <summary>
    /// Gets or sets the role of the account holder.
    /// </summary>
    [Required]
    public required string Role { get; init; }

    /// <summary>
    /// Gets or sets the password for the new account.
    /// </summary>
    [Required]
    [MinLength(6)]
    public required string Password { get; init; }

    /// <summary>
    /// Gets or sets the confirmation password for the new account. Should match Password.
    /// </summary>
    [Required]
    [Compare("Password")]
    public required string ConfirmPassword { get; init; }
}
