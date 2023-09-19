using Microsoft.EntityFrameworkCore;
using System;

namespace Ezenity_Backend.Entities.Accounts
{
    /// <summary>
    /// Represents a refresh token for an account.
    /// </summary>
    [Owned]
    public class RefreshToken
    {
        /// <summary>
        /// Gets or sets the ID of the refresh token.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the account associated with this refresh token.
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// Gets or sets the token string.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the expiration date and time of the token.
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Gets a value indicating whether the token is expired.
        /// </summary>
        public bool IsExpired => DateTime.UtcNow >= Expires;

        /// <summary>
        /// Gets or sets the date and time the token was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the IP address that created the token.
        /// </summary>
        public string CreatedByIp { get; set; }

        /// <summary>
        /// Gets or sets the date and time the token was revoked.
        /// </summary>
        public DateTime? Revoked { get; set; }

        /// <summary>
        /// Gets or sets the IP address that revoked the token.
        /// </summary>
        public string RevokedByIp { get; set; }

        /// <summary>
        /// Gets or sets the token that replaced this one.
        /// </summary>
        public string ReplacedByToken { get; set; }

        /// <summary>
        /// Gets a value indicating whether the token is active.
        /// </summary>
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
