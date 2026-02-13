namespace Ezenity.Application.Abstractions.Security;

public interface ICurrentUser
{
    int UserId { get; }
    string? Email { get; }
    bool IsAdmin { get; }
}
