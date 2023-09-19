using System;

namespace Ezenity_Backend.Models.Accounts
{
    /// <summary>
    /// Represents the response payload for account-related actions, providing details about the account.
    /// </summary>
    public class AccountResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier for the account.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title for the account holder.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the first name of the account holder.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the account holder.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email of the account holder.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the role of the account holder.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the date the account was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date the account was last updated. Null if never updated.
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the email is verified.
        /// </summary>
        public bool IsVerified { get; set; }
    }
}
