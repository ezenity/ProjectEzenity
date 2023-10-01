namespace Ezenity.Core.Interfaces
{
    public interface IPasswordService
    {
        bool VerifyPassword(string password, string hash);
    }
}
