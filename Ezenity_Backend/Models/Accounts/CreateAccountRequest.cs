using Ezenity_Backend.Entities.Accounts.v2;
using Ezenity_Backend.Models.Common.Accounts;
using System.ComponentModel.DataAnnotations;

namespace Ezenity_Backend.Models.Accounts
{
    public class CreateAccountRequest : ICreateAccountRequest
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [EnumDataType(typeof(Role))]
        public string Role { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
