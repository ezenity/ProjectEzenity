namespace Ezenity.DTOs.Models.Accounts
{
    /// <summary>
    /// Represents the request payload for revoking an authentication token.
    /// </summary>
    public class RevokeTokenRequest
    {
        /// <summary>
        /// Gets or sets the token that is to be revoked.
        /// </summary>
        public string Token { get; set; }
    }
}
