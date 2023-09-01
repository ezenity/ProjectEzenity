using System;

namespace Ezenity_Backend.Entities.Common
{
    public interface IRefreshToken
    {
        int Id { get; set; }
        IAccount Account { get; set; } 
        string Token { get; set; }
        DateTime Expires { get; set; }
        bool IsExpired { get; }
        DateTime Created { get; set; }
        string CreatedByIp { get; set; }
        DateTime? Revoked { get; set; }
        string RevokedByIp { get; set; }
        string ReplacedByToken { get; set; }
        bool IsActive { get; }
    }
}
