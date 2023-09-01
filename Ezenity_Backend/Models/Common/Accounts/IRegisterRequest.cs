namespace Ezenity_Backend.Models.Common.Accounts
{
    public interface IRegisterRequest
    {
        string Title { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string ConfirmPassword { get; set; }
        bool AcceptTerms { get; set; }
    }
}
