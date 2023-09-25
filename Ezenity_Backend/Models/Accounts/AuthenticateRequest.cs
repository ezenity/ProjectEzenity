using System.ComponentModel.DataAnnotations;

namespace Ezenity_Backend.Models.Accounts
{
    /// <summary>
    /// Represents the request payload needed for authentication.
    /// </summary>
    public class AuthenticateRequest
    {
        /// <summary>
        /// Gets or sets the email for authentication.
        /// </summary>
        [Required(ErrorMessage = "An email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password for authentication.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
