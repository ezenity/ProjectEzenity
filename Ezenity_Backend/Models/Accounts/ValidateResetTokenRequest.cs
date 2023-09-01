using Ezenity_Backend.Models.Common.Accounts;
using System.ComponentModel.DataAnnotations;

namespace Ezenity_Backend.Models.Accounts
{
    public class ValidateResetTokenRequest : IValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
