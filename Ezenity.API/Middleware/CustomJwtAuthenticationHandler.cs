using Ezenity.Core.Entities.Accounts;
using Ezenity.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Ezenity.API.Middleware
{
    /// <summary>
    /// Custom authentication handler for handling JSON Web Tokens (JWT).
    /// </summary>
    /// <remarks>
    /// This class extends the <see cref="AuthenticationHandler{TOptions}"/> to provide custom JWT authentication.
    /// It validates the JWT token from the "Authorization" header and sets the user principal accordingly.
    /// </remarks>
    public class CustomJwtAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        /// <summary>
        /// Application settings for configuring JWT.
        /// </summary>
        private readonly AppSettings _appSettings;
        private readonly DataContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomJwtAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="options">The options monitor for the authentication scheme options.</param>
        /// <param name="appSettings">The application settings for JWT.</param>
        /// <param name="logger">The logger factory.</param>
        /// <param name="encoder">The URL encoder.</param>
        /// <param name="clock">The system clock.</param>
        /// <param name="context">The data context for database operations.</param>
        public CustomJwtAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            IOptions<AppSettings> appSettings,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            DataContext context)
            : base(options, logger, encoder, clock)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }

        /// <summary>
        /// Handles the authentication asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, containing the <see cref="AuthenticateResult"/>.</returns>
        /// <remarks>
        /// This method attempts to authenticate the user by validating the JWT token from the "Authorization" header.
        /// If the token is valid, it sets the user principal accordingly and returns a successful authentication result.
        /// </remarks>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
                return AuthenticateResult.Fail("No Authorization header found.");

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

                var jwtToken = (JwtSecurityToken) validatedToken;
                var accountId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);


                var role = await GetRoleByAccountIdAsync(accountId);

                var claims = new List<Claim>
                {
                    new Claim("id", accountId.ToString()),
                    new Claim(ClaimTypes.Role, role.Name)
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while authneticating the token.");
                return AuthenticateResult.Fail("Invalid token.");
            }
        }

        private async Task<Role> GetRoleByAccountIdAsync(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            return await _context.Roles.FindAsync(account.Id);
        }
    }
}

