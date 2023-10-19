using Ezenity.Core.Interfaces;
using Ezenity.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity.API.Middleware
{
    /// <summary>
    /// Middleware for handling JSON Web Tokens (JWT) in the system.
    /// </summary>
    public class JwtMiddleware
    {
        /// <summary>
        /// The delegate for the next middleware in the pipeline.
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// Application settings for configuring JWT.
        /// </summary>
        private readonly IAppSettings _appSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtMiddleware"/> class.
        /// </summary>
        /// <param name="next">The delegate for the next middleware in the pipeline.</param>
        /// <param name="appSettings">Application settings for configuring JWT.</param>
        public JwtMiddleware(RequestDelegate next, IAppSettings appSettings)
        {
            _next = next ?? throw new ArgumentException(nameof(next));
            _appSettings = appSettings ?? throw new ArgumentException(nameof(appSettings));
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The HTTP context for the current request and response.</param>
        /// <param name="dataContext">The database context to query for accounts.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Invoke(HttpContext context, DataContext dataContext)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await attachAccountToContext(context, dataContext, token);

            await _next(context);
        }

        /// <summary>
        /// Attaches the account associated with a valid JWT to the HTTP context.
        /// </summary>
        /// <param name="context">The HTTP context for the current request and response.</param>
        /// <param name="dataContext">The database context to query for accounts.</param>
        /// <param name="token">The JWT to validate and extract account information from.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task attachAccountToContext(HttpContext context, DataContext dataContext, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var accountId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                var account = await dataContext.Accounts.FindAsync(accountId);

                context.Items["Account"] = account;
            }
            catch
            {
                // Do nothing if jwt validation fails
                // Account is not attached to context so request won't have access to secure routes
            }
        }
    }
}
