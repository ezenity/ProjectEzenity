using System;
using System.Collections.Generic;

namespace Ezenity_Backend.Entities.Accounts
{
    /// <summary>
    /// Represents an account in the system.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Gets or sets the account ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the account title.
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
        /// Gets or sets the hashed password for the account.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the terms are accepted.
        /// </summary>
        public bool AcceptTerms { get; set; }

        /// <summary>
        /// Gets or sets the Role Foreign Key
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the role of the account holder; used for navigation
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// Gets or sets the verification token for the account.
        /// </summary>
        public string VerificationToken { get; set; }

        /// <summary>
        /// Gets or sets the date and time the account was verified.
        /// </summary>
        public DateTime? Verified { get; set; }

        /// <summary>
        /// Gets a value indicating whether the account is verified.
        /// </summary>
        public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;

        /// <summary>
        /// Gets or sets the reset token for the account.
        /// </summary>
        public string ResetToken { get; set; }

        /// <summary>
        /// Gets or sets the expiration date for the reset token.
        /// </summary>
        public DateTime? ResetTokenExpires { get; set; }

        /// <summary>
        /// Gets or sets the date and time the password was reset.
        /// </summary>
        public DateTime? PasswordReset { get; set; }

        /// <summary>
        /// Gets or sets the date and time the account was created.
        /// </summary>
        public DateTime? Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time the account was last updated.
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// Gets or sets the refresh tokens for the account.
        /// </summary>
        public List<RefreshToken> RefreshTokens { get; set; }

        /// <summary>
        /// Checks if the account owns a specific token.
        /// </summary>
        /// <param name="token">The token to check ownership of.</param>
        /// <returns>true if the account owns the token, false otherwise.</returns>
        public bool OwnsToken(string token)
        {
            return this.RefreshTokens?.Find(x => x.Token == token) != null;
        }
    }
}
