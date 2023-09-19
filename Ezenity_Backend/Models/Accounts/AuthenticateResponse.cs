using System;
using System.Text.Json.Serialization;

namespace Ezenity_Backend.Models.Accounts
{
    /// <summary>
    /// Represents the response payload for successful authentication.
    /// </summary>
    public class AuthenticateResponse
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
        /// Gets or sets the date when the account was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date when the account was last updated. Null if never updated.
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the account's email has been verified.
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Gets or sets the JSON Web Token (JWT) for authenticated sessions.
        /// </summary>
        public string JwtToken { get; set; }

        /// <summary>
        /// Gets or sets the refresh token. This property will not be serialized.
        /// </summary>
        [JsonIgnore] // Refresh token is returned in http only cookie
        public string RefreshToken { get; set; }
    }
}
