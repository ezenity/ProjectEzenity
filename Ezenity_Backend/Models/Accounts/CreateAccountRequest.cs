using Ezenity_Backend.Entities.Accounts;
using System.ComponentModel.DataAnnotations;

namespace Ezenity_Backend.Models.Accounts
{
    /// <summary>
    /// Represents the request payload for creating a new account.
    /// </summary>
    public class CreateAccountRequest
    {
        /// <summary>
        /// Gets or sets the title for the account holder.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the first name of the account holder.
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the account holder.
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email of the account holder.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the role of the account holder.
        /// </summary>
        [Required]
        [EnumDataType(typeof(Role))]
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the password for the new account.
        /// </summary>
        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the confirmation password for the new account. Should match Password.
        /// </summary>
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
