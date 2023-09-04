using System;
using System.Collections.Generic;

namespace Ezenity_Backend.Entities.Common
{
    public interface IAccount
    {
        int Id { get; set; }
        string Title { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string PasswordHash { get; set; }
        bool AcceptTerms { get; set; }
        IRole Role { get; set; }
        string VerificationToken { get; set; }
        DateTime? Verified { get; set; }
        bool IsVerified { get; }
        string ResetToken { get; set; }
        DateTime? ResetTokenExpires { get; set; }
        DateTime? PasswordReset { get; set; }
        DateTime? Created { get; set; }
        DateTime? Updated { get; set; }
        List<IRefreshToken> RefreshTokens { get; set; }

        bool OwnsToken(string token);
    }
}
