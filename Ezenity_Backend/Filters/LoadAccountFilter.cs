using Ezenity_Backend.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Ezenity_Backend.Filters
{
    public class LoadAccountFilter : IAsyncActionFilter
    {
        private readonly DataContext _context;

        public LoadAccountFilter(DataContext context)
        {
            _context = context;
        }

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
