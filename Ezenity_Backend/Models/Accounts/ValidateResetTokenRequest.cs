using System.ComponentModel.DataAnnotations;

namespace Ezenity_Backend.Models.Accounts
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
