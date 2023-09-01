namespace Ezenity_Backend.Models.Common.Accounts
{
    public interface IValidateResetTokenRequest
    {
        string Token { get; set; }
    }
}
