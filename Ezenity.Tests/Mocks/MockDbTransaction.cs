using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ezenity.Tests.Mocks
{
    /// <summary>
    /// A mock implementation of IDbContextTransaction for unit testing purposes.
    /// </summary>
    public class MockDbTransaction : IDbContextTransaction
    {
        /// <summary>
        /// Gets the transaction identifier.
        /// </summary>
        public Guid TransactionId { get; } = Guid.NewGuid();

        /// <summary>
        /// Simulates the Commit action for the transaction.
        /// </summary>
        public void Commit()
        {
            Console.WriteLine("Commit was called.");
        }

        /// <summary>
        /// Simulates the asychronous Commit action for the transaction.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        public Task CommitAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("CommitAsync was called.");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Disposes of the transaction resources.
        /// </summary>
        public void Dispose()
        {
            Console.WriteLine("Dispose was called.");
        }

        /// <summary>
        /// Asynchronously disposes of the transation resources.
        /// </summary>
        /// <returns></returns>
        public ValueTask DisposeAsync()
        {
            Console.WriteLine("DisposeAsync was called.");
            return new ValueTask(Task.CompletedTask);
        }

        /// <summary>
        /// Simulates the Rollback action for the transaction.
        /// </summary>
        public void Rollback()
        {
            Console.WriteLine("Rollback was called.");
        }

        /// <summary>
        /// Simulates the asynchronous Rollback action for the transaction.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        public Task RollbackAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("RollbackAsync was called.");
            return Task.CompletedTask;
        }
    }
}
