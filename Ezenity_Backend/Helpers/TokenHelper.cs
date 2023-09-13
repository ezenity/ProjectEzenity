using Ezenity_Backend.Entities.Accounts;
using Ezenity_Backend.Helpers.Exceptions;
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

namespace Ezenity_Backend.Helpers
{
    public class TokenHelper
    {
        private readonly DataContext _context;
        private readonly AppSettings _appSettings;
        private readonly string _secret;

        public TokenHelper(DataContext context, IOptions<AppSettings> appSettings, string secret)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _secret = secret;
        }

        public TokenHelper(string secret)
        {
            _secret = secret;
        }

        public string GenerateJwtToken(int accountId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, accountId.ToString())
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
        /// Generates a new refresh token for the given IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address initiating the request.</param>
        /// <returns>The newly generated refresh token.</returns>
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

        public string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new Byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // Convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }
    }
}
