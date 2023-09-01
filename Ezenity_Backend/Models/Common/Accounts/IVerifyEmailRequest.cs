namespace Ezenity_Backend.Models.Common.Accounts
{
    public interface IVerifyEmailRequest
    {
        string Token { get; set; }
    }
}
