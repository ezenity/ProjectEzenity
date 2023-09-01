using System;

namespace Ezenity_Backend.Models.Common.Accounts
{
    public interface IAuthenticateResponse
    {
        int Id { get; set; }
        string Title { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string Role { get; set; }
        DateTime Created { get; set; }
        DateTime? Updated { get; set; }
        bool IsVerified { get; set; }
        string JwtToken { get; set; }
        string RefreshToken { get; set; }
    }
}
