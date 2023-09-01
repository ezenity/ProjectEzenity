namespace Ezenity_Backend.Models.Common.Accounts
{
    public interface IResetPasswordRequest
    {
        string Token { get; set; }
        string Password { get; set; }
        string ConfirmPassword { get; set; }
    }
}
