namespace Ezenity_Backend.Services.Common
{
    public interface IAuthService
    {
        int GetCurrentUserId();
        bool IsCurrentUserAdmin();
        string GetCurrentUserEmail();
    }
}
