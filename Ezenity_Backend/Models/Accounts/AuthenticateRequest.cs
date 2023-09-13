using System.ComponentModel.DataAnnotations;

namespace Ezenity_Backend.Models.Accounts
{
    public class AuthenticateRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
