namespace Ezenity_Backend.Services.Common
{
    /// <summary>
    /// Provides utility functions for authorization within the application.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Gets the current user's ID.
        /// </summary>
        int GetCurrentUserId();

        /// <summary>
        /// Checks if the current user is an admin.
        /// </summary>
        bool IsCurrentUserAdmin();

        /// <summary>
        /// Gets the email of the current user.
        /// </summary>
        string GetCurrentUserEmail();
    }
}
