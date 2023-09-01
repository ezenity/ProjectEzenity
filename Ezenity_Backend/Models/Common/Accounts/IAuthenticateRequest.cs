namespace Ezenity_Backend.Models.Common.Accounts
{
    public interface IAuthenticateRequest
    {
        string Email { get; set; }
        string Password { get; set; }
    }
}
