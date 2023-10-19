using Ezenity.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Ezenity.API.Filters
{
    /// <summary>
    /// Represents a filter that loads an account from the database based on the "Account" HTTP header.
    /// This class implements the <see cref="IAsyncActionFilter"/> interface.
    /// </summary>
    public class LoadAccountFilter : IAsyncActionFilter
    {
        /// <summary>
        /// The database context used for retrieving the account data.
        /// </summary>
        private readonly IDataContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadAccountFilter"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public LoadAccountFilter(IDataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Executes the action filter asynchronously.
        /// </summary>
        /// <param name="context">The context for the action, including the HTTP context and action arguments.</param>
        /// <param name="next">The delegate for executing the next stage in the filter pipeline.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        /// <remarks>
        /// This method tries to parse the "Account" HTTP header into an integer (account ID),
        /// and then queries the database for the corresponding account. If the account exists,
        /// it is stored in the HttpContext.Items dictionary for later use.
        /// </remarks>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var accountIdHeader = context.HttpContext.Request.Headers["Account"].ToString();

            if(int.TryParse(accountIdHeader, out var accountId))
            {
                var account = await _context.Accounts.SingleOrDefaultAsync(a => a.Id == accountId);

                if (account == null)
                    context.HttpContext.Items["Entity"] = account;
            }

            await next();
        }
    }
}
