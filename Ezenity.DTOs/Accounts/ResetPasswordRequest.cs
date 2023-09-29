using System.ComponentModel.DataAnnotations;

namespace Ezenity.DTOs.Models.Accounts
{
    /// <summary>
    /// Represents the request payload for resetting the account password.
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// Gets or sets the verification token.
        /// </summary>
        [Required]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the role of the account holder.
        /// </summary> 
        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the confirmation password for the account. Should match Password.
        /// </summary>
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
