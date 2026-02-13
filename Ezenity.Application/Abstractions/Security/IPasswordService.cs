namespace Ezenity.Application.Abstractions.Security;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
