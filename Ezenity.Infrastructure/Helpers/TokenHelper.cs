using Ezenity.Core.Entities.Accounts;
using Ezenity.Core.Helpers.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ezenity.Core.Interfaces;

namespace Ezenity.Infrastructure.Helpers
{
    /// <summary>
    /// Provides functionalities for handling JWT and refresh tokens.
    /// 
    /// One instance of 'TokenHelper' per HTTP request. This will ensure
    /// that it is working with a consistent set of data within a single 
    /// request. This will also keep some state consistent throughout
    /// the single request lifecycle but will not keep the state longer
    /// than that.
    /// </summary>
    public class TokenHelper : ITokenHelper
    {
        /// <summary>
        /// Data context for database interaction.
        /// </summary>
        private readonly IDataContext _context;

        /// <summary>
        /// Application-specific settings.
        /// </summary>
        private readonly AppSettings _appSettings;


        /// <summary>
        /// Initializes a new instance of the TokenHelper class with DataContext, AppSettings, and secret string.
        /// </summary>
        /// <param name="context">The data context for database interaction.</param>
        /// <param name="appSettings">Application-specific settings.</param>
        /// <param name="secret">Secret key for token generation.</param>
        public TokenHelper(IDataContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Initializes a new instance of the TokenHelper class with only a secret string.
        /// </summary>
        /// <param name="secret">Secret key for token generation.</param>
/*        public TokenHelper(string secret)
        {
            _secret = secret;
        }*/

        /// <summary>
        /// Generates a JWT token for a given account ID.
        /// </summary>
        /// <param name="accountId">The account ID for which to generate the token.</param>
        /// <returns>The JWT token as a string.</returns>
        public string GenerateJwtToken(int accountId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var claims = new[]
            {
                new Claim("id", accountId.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Generates a new refresh token.
        /// </summary>
        /// <param name="ipAddress">The originating IP address.</param>
        /// <returns>A new RefreshToken object.</returns>
        public RefreshToken GenerateNewRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                //Expires = DateTime.UtcNow.AddSeconds(15),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        /// <summary>
        /// Updates the given refresh token with revocation information and adds a new refresh token to the associated account.
        /// </summary>
        /// <param name="refreshToken">The refresh token to revoke.</param>
        /// <param name="newRefreshToken">The new refresh token to add.</param>
        /// <param name="ipAddress">The IP address initiating the request.</param>
        public void UpdateRefreshToken(RefreshToken refreshToken, RefreshToken newRefreshToken, string ipAddress)
        {
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            refreshToken.Account.RefreshTokens.Add(newRefreshToken);
        }

        /// <summary>
        /// Removes old or expired refresh tokens from an account.
        /// </summary>
        /// <param name="account">The account for which to remove old refresh tokens.</param>
        public void RemoveOldRefreshTokens(Account account)
        {
            account.RefreshTokens.RemoveAll(x => !x.IsActive && x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        /// <summary>
        /// Retrieves the refresh token and associated account for the given token string.
        /// </summary>
        /// <param name="token">The token string to search for.</param>
        /// <returns>A tuple containing the refresh token and associated account.</returns>
        /// <exception cref="AppException">Thrown if the token is invalid or inactive.</exception>
        public async Task<(RefreshToken, Account)> GetRefreshTokenAsync(string token)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (account == null) throw new AppException("Invalid Token");
            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive) throw new AppException("Invalid token.");
            return (refreshToken, account);
        }

        /// <summary>
        /// Generates a random token string.
        /// </summary>
        /// <returns>The generated token string.</returns>
        public string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            //var test = new RandomNumberGenerator().Create();
            var randomBytes = new Byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // Convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }
    }
}
