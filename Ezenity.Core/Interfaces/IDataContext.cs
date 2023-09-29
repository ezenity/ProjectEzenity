using Ezenity.Core.Entities.Accounts;
using Ezenity.Core.Entities.EmailTemplates;
using Ezenity.Core.Entities.Sections;
using Microsoft.EntityFrameworkCore;

namespace Ezenity.Core.Interfaces
{
    public interface IDataContext : IDisposable
    {
        DbSet<Account> Accounts { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<EmailTemplate> EmailTemplates { get; set; }
        DbSet<Section> Sections { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
