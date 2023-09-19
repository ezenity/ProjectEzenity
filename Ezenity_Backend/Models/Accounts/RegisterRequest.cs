using System;
using System.ComponentModel.DataAnnotations;

namespace Ezenity_Backend.Models.Accounts
{
    /// <summary>
    /// Represents the request payload for account registration.
    /// </summary>
    public class RegisterRequest
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
        [MinLength(6)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the confirmation password for the new account. Should match Password.
        /// </summary>
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user accepts the terms and conditions.
        /// </summary>
        [Range(typeof(bool), "true", "true")]
        public bool AcceptTerms { get; set; }
    }
}
