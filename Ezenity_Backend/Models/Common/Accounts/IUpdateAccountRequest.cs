namespace Ezenity_Backend.Models.Common.Accounts
{
    public interface IUpdateAccountRequest
    {
        string Title { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Role { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string ConfirmPassword { get; set; }
    }
}
