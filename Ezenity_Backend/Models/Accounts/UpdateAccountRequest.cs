using Ezenity_Backend.Entities.Accounts;
using System.ComponentModel.DataAnnotations;

namespace Ezenity_Backend.Models.Accounts
{
    /// <summary>
    /// Represents the request payload for updating an account's information.
    /// </summary>
    public class UpdateAccountRequest
    {
        private string _password;
        private string _confirmPassword;
        private string _role;
        private string _email;

        /// <summary>
        /// Gets or sets the title for the account holder.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the first name of the account holder.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the account holder.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the role ID of the account holder. This field is optional.
        /// </summary>
        public string? Role
        {
            get => _role;
            set => _role = replaceEmptyWithNull(value);
        }

        /// <summary>
        /// Gets or sets the email of the account holder. This field is optional.
        /// </summary>
        [EmailAddress]
        public string? Email
        {
            get => _email;
            set => _email = replaceEmptyWithNull(value);
        }

        /// <summary>
        /// Gets or sets the password of the account holder. This field is optional.
        /// </summary>
        [MinLength(6)]
        public string? Password
        {
            get => _password;
            set => _password = replaceEmptyWithNull(value);
        }

        /// <summary>
        /// Gets or sets the confirmation password of the account holder. Should match the password.
        /// </summary>
        [Compare("Password")]
        public string? ConfirmPassword
        {
            get => _confirmPassword;
            set => _confirmPassword = replaceEmptyWithNull(value);
        }

        // Helper Methods

        /// <summary>
        /// Replaces an empty string with null, making the field optional.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <returns>Returns null if the string is empty; otherwise, returns the original string.</returns>
        private string replaceEmptyWithNull(string value)
        {
            // Replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
