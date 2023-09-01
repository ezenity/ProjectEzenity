using Ezenity_Backend.Models.Common.Accounts;

namespace Ezenity_Backend.Models.Accounts
{
    public class RevokeTokenRequest : IRevokeTokenRequest
    {
        public string Token { get; set; }
    }
}
