using Ezenity.Application.Abstractions.Security;
using BC = BCrypt.Net.BCrypt;

namespace Ezenity.Infrastructure.Security;

public sealed class PasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        // We can tune work factor later; default is okay.
        return BC.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;

        return BC.Verify(password, hash);
    }
}
