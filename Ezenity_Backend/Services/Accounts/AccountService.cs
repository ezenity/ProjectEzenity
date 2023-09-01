using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ezenity_Backend.Entities;
using Ezenity_Backend.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;
using Ezenity_Backend.Models.Common.Accounts;
using Ezenity_Backend.Services.Common;

namespace Ezenity_Backend.Services.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountService> _logger;
        private readonly TokenHelper _tokenHelper;

        public AccountService(DataContext context, IMapper mapper, IOptions<AppSettings> appSettings, IEmailService emailService, ILogger<AccountService> logger, TokenHelper tokenHelper)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _logger = logger;
            _tokenHelper = tokenHelper;
        }

        public IAccountResponse GetById(int id)
        {
            //GetByIdAsync(id).Wait();
            return GetByIdAsync(id).GetAwaiter().GetResult();
            //throw new NotImplementedException();
        }

        public IAccountResponse Create(ICreateAccountRequest model)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccountResponse> GetAll()
        {
            throw new NotImplementedException();
        }

        public IAccountResponse Update(int id, IUpdateAccountRequest model)
        {
            throw new NotImplementedException();
        }

        public async Task<IAuthenticateResponse> AuthenticateAsync(IAuthenticateRequest model, string ipAddress)
        {
            IAccount account = null;
            string accountVersion = null;

            try
            {
                if(await _context.AccountsV1.AnyAsync(x => x.Email == model.Email))
                {
                    account = await _context.AccountsV1.SingleOrDefaultAsync(x => x.Email == model.Email);
                    accountVersion = "v1";
                }
                else if(await _context.AccountsV2.AnyAsync(x => x.Email == model.Email))
                {
                    account = await _context.AccountsV2.SingleOrDefaultAsync(x => x.Email == model.Email);
                    accountVersion = "v2";
                }
                //var account = await _context.AccountsV1.SingleOrDefaultAsync(x => x.Email == model.Email);
                //var account = await _context.AccountsV2.SingleOrDefaultAsync(x => x.Email == model.Email);
                //var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

                if (account == null || !account.IsVerified)
                    throw new AppException("The Email, {0}, was not found", account.Email);

                if (!BC.Verify(model.Password, account.PasswordHash))
                    throw new AppException("The 'password' provided is incorrect");

                // Authentication successful so generate jwt and refresh tokens
                var jwtToken = _tokenHelper.GenerateJwtToken(account.Id, accountVersion);
                var refreshtoken = generateRefreshToken(ipAddress);

                account.RefreshTokens.Add(refreshtoken);

                // Remove old refresh tokens from account
                removeOldRefreshTokens(account);

                // Save changes to DB
                _context.Update(account);
                await _context.SaveChangesAsync();

                var response = _mapper.Map<IAuthenticateResponse>(account);
                response.JwtToken = jwtToken;
                response.RefreshToken = refreshtoken.Token;

                return response;
            }
            catch (Exception ex)
            {
                // TODO: Log the exception
                throw new AppException("An error occurred while authenticating the user.", ex);
            }
        }

        public async Task<IAuthenticateResponse> RefreshTokenAsync(string token, string ipAddress)
        {
            try
            {
                var (refrehToken, account) = await getRefreshTokenAsync(token);

                string version;

                if (account is Entities.Accounts.v1.Account)
                    version = "v1";
                else if (account is Entities.Accounts.v2.Account)
                    version = "v2";
                else
                    throw new AppException("Unknown account version");

                // Replace old refresh token with a new one and save
                var newRefreshToken = generateRefreshToken(ipAddress);
                refrehToken.Revoked = DateTime.UtcNow;
                refrehToken.RevokedByIp = ipAddress;
                refrehToken.ReplacedByToken = newRefreshToken.Token;
                account.RefreshTokens.Add(newRefreshToken);

                removeOldRefreshTokens(account);

                _context.Update(account);
                await _context.SaveChangesAsync();

                // Generate new JWT
                var jwtToken = _tokenHelper.GenerateJwtToken(account.Id, version);

                var response = _mapper.Map<IAuthenticateResponse>(account);
                response.JwtToken = jwtToken;
                response.RefreshToken = newRefreshToken.Token;
                return response;
            }
            catch (Exception ex)
            {
                throw new AppException("An error occurred while refreshing account token.", ex);
            }
        }

        public async Task RevokeTokenAsync(string token, string ipAddress)
        {
            var (refreshToken, account) = await getRefreshTokenAsync(token);

            // Revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _context.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task RegisterAsync(IRegisterRequest model, string origin)
        {
            try
            {
                // Check is account exists
                if (!AccountLookUpVersion(model).Result.Email.Equals(null))
                    await _emailService.SendEmailAsync(await AccountAlreadyRegisteredAsync(model));

                // Map model to new account object
                var account = _mapper.Map<IAccount>(model);

                var version = await CheckAccountVersion(model.Email);

                if (version.Equals("V1"))
                    account.Role = IsFirstAccount(model) ? new Entities.Accounts.v1.RoleWrapper(Entities.Accounts.v1.Role.Admin) : new Entities.Accounts.v1.RoleWrapper(Entities.Accounts.v1.Role.User);
                else if (version.Equals("V2"))
                    account.Role = IsFirstAccount(model) ? new Entities.Accounts.v2.Role { Name = "Admin" } : new Entities.Accounts.v2.Role { Name = "User" };

                account.Created = DateTime.UtcNow;
                account.VerificationToken = _tokenHelper.RandomTokenString();

                // Hash password
                account.PasswordHash = BC.HashPassword(model.Password);

                // Save account
                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                // Send email
                //sendVerificationEmail(account, origin);
                var verifyEmail = new EmailMessage
                {
                    To = model?.Email,
                    TemplateName = "verification"
                };

                await _emailService.SendEmailAsync(verifyEmail);
            }
            catch (Exception ex)
            {
                throw new AppException("An error occurred while registering the user.", ex);
            }
        }

        public async Task VerifyEmailAsync(string token)
        {
            try
            {
                var account = await _context.Accounts.SingleOrDefaultAsync(x => x.VerificationToken == token);

                if (account == null) throw new AppException("Verification failed.");

                account.Verified = DateTime.UtcNow;
                account.VerificationToken = null;

                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new AppException("An error occurred while verifing the accounts email.", ex);
            }
        }

        public async Task ForgotPasswordAsync(IForgotPasswordRequest model, string origin)
        {
            try
            {
                var account = await _context.Accounts.SingleOrDefaultAsync(x => x.Email == model.Email);

                // Always return ok response to prevent email enumeration
                if (account == null) return;

                // Create reset token that expires after 1 day
                account.ResetToken = _tokenHelper.RandomTokenString();
                account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();

                // URL-encode the token
                var encodedToken = System.Net.WebUtility.UrlEncode(account.ResetToken);

                // Construct reset password URL with token
                var resetUrl = $"{origin}/account/reset-password?token={encodedToken}";

                // Send email
                var resetEmail = new EmailMessage
                {
                    To = model?.Email,
                    TemplateName = "passwordReset",
                    DynamicValues = new Dictionary<string, string>
                {
                    { "firstNameValue", account.FirstName },
                    { "token", encodedToken },
                    { "resetUrl", resetUrl }
                }
                };
                await _emailService.SendEmailAsync(resetEmail);
            }
            catch (Exception ex)
            {
                throw new AppException("An error occurred while attempting to reset the accounts password.", ex);
            }
        }

        public async Task ValidateResetTokenAsync(IValidateResetTokenRequest model)
        {
            try
            {
                var account = await _context.Accounts.SingleOrDefaultAsync(x =>
                    x.ResetToken == model.Token &&
                    x.ResetTokenExpires > DateTime.UtcNow
                );

                if (account == null)
                    throw new AppException("Invalid token.");
            }
            catch (Exception ex)
            {
                throw new AppException("An error occurred while validating the accounts reset token.", ex);
            }
        }

        public async Task ResetPasswordAsync(IResetPasswordRequest model)
        {
            try
            {
                var account = await _context.Accounts.SingleOrDefaultAsync(x =>
                    x.ResetToken == model.Token &&
                    x.ResetTokenExpires > DateTime.UtcNow
                );

                if (account == null)
                    throw new AppException("Invalid token.");

                // Update password and remove reset token
                account.PasswordHash = BC.HashPassword(model.Password);
                account.PasswordReset = DateTime.UtcNow;
                account.ResetToken = null;
                account.ResetTokenExpires = null;

                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new AppException("An error occurred while resetting the accounts password.", ex);
            }
        }

        public async Task<IEnumerable<IAccountResponse>> GetAllAsync()
        {
            try
            {
                var accounts = await _context.Accounts
                                        .ProjectTo<AccountResponse>(_mapper.ConfigurationProvider)
                                        .ToListAsync();
                return accounts;
            }
            catch (Exception ex)
            {
                throw new AppException("An error occurred while getting the accounts.", ex);
            }
        }

        public async Task<IAccountResponse> GetByIdAsync(int id)
        {
            try
            {
                var account = await _context.Accounts
                                        .Where(x => x.Id == id)
                                        .ProjectTo<AccountResponse>(_mapper.ConfigurationProvider)
                                        .SingleOrDefaultAsync();
                return account;
            }
            catch (Exception ex)
            {
                throw new AppException("An error occurred while getting the account by id.", ex);
            }
        }

        public async Task<IAccountResponse> CreateAsync(ICreateAccountRequest model)
        {
            try
            {
                // Validate
                if (await _context.Accounts.AnyAsync(x => x.Email == model.Email))
                    throw new AppException($"Email '{model.Email}' is already registered");

                // Map model to new account object
                var account = _mapper.Map<Account>(model);
                account.Created = DateTime.UtcNow;
                account.Verified = DateTime.UtcNow;

                // Hash password
                account.PasswordHash = BC.HashPassword(model.Password);

                // Save account
                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                return _mapper.Map<AccountResponse>(account);
            }
            catch (Exception ex)
            {
                throw new AppException("An error occurred while creating an account.", ex);
            }
        }

        public async Task<IAccountResponse> UpdateAsync(int id, IUpdateAccountRequest model)
        {
            try
            {
                var account = await getAccountAsync(id);

                // Validate
                if (account.Email != model.Email && await _context.Accounts.AnyAsync(x => x.Email == model.Email))
                    throw new AppException($"Email '{model.Email}' is already taken");

                // Hash password if it was entered
                if (!string.IsNullOrEmpty(model.Password))
                    account.PasswordHash = BC.HashPassword(model.Password);

                // Copy model to accound and save
                _mapper.Map(model, account);
                account.Updated = DateTime.UtcNow;
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();

                return _mapper.Map<AccountResponse>(account);
            }
            catch (Exception ex)
            {
                throw new AppException("An error occurred while updating an account.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var account = await getAccountAsync(id);
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new AppException("An error occurred while deleting an account.", ex);
            }
        }

        /// //////////////////
        /// Helper Methods ///
        /// //////////////////

        private async Task<IAccount> getAccountAsync(int id)
        {
            try
            {
                var account = await _context.Accounts.FindAsync(id);
                if (account == null) throw new KeyNotFoundException("Account not found.");
                return account;
            }
            catch (Exception ex)
            {
                throw new AppException("An error occurred while obtaining an account by id.", ex);
            }
        }

        private async Task<(IRefreshToken, IAccount)> getRefreshTokenAsync(string token)
        {
            try
            {
                var account = await _context.Accounts.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
                if (account == null) throw new AppException("Invalid Token");
                var refreshToken = account.RefreshTokens.Single(x => x.Token == token);
                if (!refreshToken.IsActive) throw new AppException("Invalid token.");
                return (refreshToken, account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the refresh token");
                throw new AppException("An error occurred. Please try again later.", ex);
            }
        }

        private IRefreshToken generateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = _tokenHelper.RandomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                //Expires = DateTime.UtcNow.AddSeconds(15),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private void removeOldRefreshTokens(IAccount account)
        {
            account.RefreshTokens.RemoveAll(x => !x.IsActive && x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private async Task<bool> IsAccountFoundAsync(IRegisterRequest model)
        {
            if (await _context.AccountsV1.AnyAsync(x => x.Email == model.Email))
                return true;
            else if (await _context.AccountsV2.AnyAsync(x => x.Email == model.Email))
                return true;

            return false;
        }

        private async Task<IRegisterRequest> AccountLookUpVersion(IRegisterRequest model)
        {
            if (await _context.AccountsV1.AnyAsync(x => x.Email == model.Email))
                return model;
            else if (await _context.AccountsV2.AnyAsync(x => x.Email == model.Email))
                return model;

            return model;
        }

        private bool IsFirstAccount(IRegisterRequest model)
        {
            if (_context.AccountsV1.Count().Equals(0))
                return true;
            else if (_context.AccountsV2.Count().Equals(0))
                return true;
            else
                return false;
        }

        private async Task<string> CheckAccountVersion(string email)
        {
            if (await _context.AccountsV1.AnyAsync(x => x.Email == email))
                return "V1";
            else if (await _context.AccountsV2.AnyAsync(x => x.Email == email))
                return "V2";

            throw new AppException("No matching emai lfound in any context version");
        }

        private async Task<IEmailMessage> AccountAlreadyRegisteredAsync(IRegisterRequest model)
        {
            //string accountVersion;
            IEmailMessage alreadyRegisteredEmail; // This is the base type that both email types inherit from

            // For v1 accounts
            if (await _context.AccountsV1.AnyAsync(x => x.Email == model.Email))
            {
                alreadyRegisteredEmail = new Entities.v1.Emails.EmailMessage
                {
                    To = model.Email,
                    TemplateName = "alreadyRegistered"
                };
            }
            // For v2 accounts
            else if (await _context.AccountsV2.AnyAsync(x => x.Email == model.Email))
            {
                alreadyRegisteredEmail = new Entities.v2.Emails.Message
                {
                    To = model.Email,
                    TemplateName = "alreadyRegistered"
                };
            }
            else
            {
                alreadyRegisteredEmail = null;
            }

            return alreadyRegisteredEmail;
        }


    }
}
