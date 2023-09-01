using Ezenity_Backend.Entities.Accounts;
using Ezenity_Backend.Models.Common.Accounts;
using System.ComponentModel.DataAnnotations;

namespace Ezenity_Backend.Models.Accounts
{
    public class UpdateAccountRequest : IUpdateAccountRequest
    {
        private string _password;
        private string _confirmPassword;
        private string _role;
        private string _email;

        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [EnumDataType(typeof(Role))]
        public string Role
        {
            get => _role;
            set => _role = replaceEmptyWithNull(value);
        }

        [EmailAddress]
        public string Email
        {
            get => _email;
            set => _email = replaceEmptyWithNull(value);
        }

        [MinLength(6)]
        public string Password
        {
            get => _password;
            set => _password = replaceEmptyWithNull(value);
        }

        [Compare("Password")]
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => _confirmPassword = replaceEmptyWithNull(value);
        }

        // Helper Methods

        private string replaceEmptyWithNull(string value)
        {
            // Replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
