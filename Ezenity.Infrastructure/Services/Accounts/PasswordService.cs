using Ezenity.Core.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace Ezenity.Infrastructure.Services.Accounts
{
    public class PasswordService : IPasswordService
    {
        public bool VerifyPassword(string password, string hash)
        {
            return BC.Verify(password, hash);
        }
    }
}
