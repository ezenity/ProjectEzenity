using Ezenity.Core.Entities.Accounts;
using Ezenity.Core.Entities.EmailTemplates;
using Ezenity.Core.Entities.Sections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ezenity.Core.Interfaces
{
    /// <summary>
    /// Provides the contract for the application's data context.
    /// </summary>
    public interface IDataContext : IDisposable
    {
        /// <summary>
        /// Gets or sets the DbSet representing the Accounts in the application.
        /// </summary>
        DbSet<Account> Accounts { get; set; }

        /// <summary>
        /// Gets or sets the DbSet representing the Roles in the application.
        /// </summary>
        DbSet<Role> Roles { get; set; }

        /// <summary>
        /// Gets or sets the DbSet representing the Email Templates in the application.
        /// </summary>
        DbSet<EmailTemplate> EmailTemplates { get; set; }

        /// <summary>
        /// Gets or sets the DbSet representing the Sections in the application.
        /// </summary>
        DbSet<Section> Sections { get; set; }

        /// <summary>
        /// Saves all changes made in this context to the underlying database synchronously.
        /// </summary>
        /// <returns>The number of state entries written to the underlying database.</returns>
        int SaveChanges();

        /// <summary>
        /// Saves all changes made in this context to the underlying database asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the underlying database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins a new transaction synchronously.
        /// </summary>
        /// <returns>A transaction object representing the new transaction.</returns>
        IDbContextTransaction BeginTransaction();

        /// <summary>
        /// Begins a new transaction asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the new transaction object.</returns>
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks the given entity as modified.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>The EntityEntry for the entity. The returned entry provides access to change tracking information and operations for the entity.</returns>
        EntityEntry Update(object entity);
    }
}
