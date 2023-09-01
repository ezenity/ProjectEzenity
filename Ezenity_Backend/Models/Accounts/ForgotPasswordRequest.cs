using Ezenity_Backend.Models.Common.Accounts;
using System.ComponentModel.DataAnnotations;

namespace Ezenity_Backend.Models.Accounts
{
    public class ForgotPasswordRequest : IForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
