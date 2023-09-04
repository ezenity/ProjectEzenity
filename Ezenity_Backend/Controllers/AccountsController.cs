using Ezenity_Backend.Entities.Accounts;
using Ezenity_Backend.Entities.Common;
using Ezenity_Backend.Helpers;
using Ezenity_Backend.Models;
using Ezenity_Backend.Models.Common.Accounts;
using Ezenity_Backend.Services.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ezenity_Backend.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : BaseController<IAccount, IAccountResponse, ICreateAccountRequest, IUpdateAccountRequest>
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountsController> _logger;

        // Property for the current authenticated account
        public IAccount CurrentAccount => (IAccount)HttpContext.Items["Account"];

        public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        // Common CRUD methods
        [HttpGet("{id:int}")]
        public override async Task<ActionResult<IAccountResponse>> GetByIdAsync(int id)
        {
            try
            {
                var account = await _accountService.GetByIdAsync(id);

                if (account == null)
                {
                    return NotFound(new ApiResponse<IAccountResponse>
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = "Account not found."
                    });
                }

                return Ok(new ApiResponse<IAccountResponse>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "Account fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error] Account fetched unsuccessfully: {0}", ex);

                return StatusCode(500, new ApiResponse<IAccountResponse>
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = "An error occurred while fetching the Account."
                });
            }
        }

        [Authorize("Admin")]
        [HttpGet]
        public override async Task<ActionResult<IEnumerable<IAccountResponse>>> GetAllAsync()
        {
            try
            {
                var accounts = await _accountService.GetAllAsync();

                if (accounts == null)
                {
                    return NotFound(new ApiResponse<IAccountResponse>
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = "Accounts not found."
                    });
                }

                return Ok(new ApiResponse<IAccountResponse>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "Accounts fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error] Accounts fetched unsuccessfully: {0}", ex);

                return StatusCode(500, new ApiResponse<IAccountResponse>
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = "An error occurred while fetching the Accounts."
                });
            }
        }

        [Authorize("Admin")]
        [HttpPost]
        public override async Task<ActionResult<IAccountResponse>> CreateAsync(ICreateAccountRequest model)
        {
            try
            {
                var account = await _accountService.CreateAsync(model);

                if (account.Email == model.Email)
                {
                    return Conflict(new ApiResponse<IAccountResponse>
                    {
                        StatusCode = 409,
                        IsSuccess = false,
                        Message = "An Account with this email already exists."
                    });
                }

                return Created(""/* TODO: input API endpoint to get account details (URL) */, new ApiResponse<IAccountResponse>
                {
                    StatusCode = 201,
                    IsSuccess = true,
                    Message = "Account created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error] Accounts created unsuccessfully: {0}", ex);

                return StatusCode(500, new ApiResponse<IAccountResponse>
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = "An error occurred while creating the Account."
                });
            }
        }

        [Authorize("Admin")]
        [HttpPut("{id:int}")]
        public override async Task<ActionResult<IAccountResponse>> UpdateAsync(int id, IUpdateAccountRequest model)
        {
            try
            {
                // Users can update their own account and admins can update any account
                if (!IsAccountId(id))
                    return Unauthorized(new { message = "Unauthorized" });

                var account = await _accountService.UpdateAsync(id, model);

                if (account.Email == model.Email)
                {
                    return Conflict(new ApiResponse<IAccountResponse>
                    {
                        StatusCode = 409,
                        IsSuccess = false,
                        Message = "An Account with this email already exists."
                    });
                }

                return Ok(new ApiResponse<IAccountResponse>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "Account created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error] Accounts created unsuccessfully: {0}", ex);

                return StatusCode(500, new ApiResponse<IAccountResponse>
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = "An error occurred while creating the Account."
                });
            }
        }

        [Authorize("Admin")]
        [HttpDelete("{id:int}")]
        public override async Task<IActionResult> DeleteAsync(int id)
        {
            // Users can delete their own account and admins can delete any account
            if (!IsAccountId(id))
                return Unauthorized(new { message = "Unauthorized" });
            await _accountService.DeleteAsync(id);
            return Ok(new { message = "Account deleted seuccessfully." });
        }

        // Uncommon Methods

        [HttpPost("authenticate")]
        public async Task<ActionResult<IAuthenticateResponse>> AuthenticateAsync(IAuthenticateRequest model)
        {
            var response = await _accountService.AuthenticateAsync(model, ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<IAuthenticateResponse>> RefreshTokenAsync()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _accountService.RefreshTokenAsync(refreshToken, ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [Authorize("Admin")]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeTokenAsync(IRevokeTokenRequest model)
        {
            // Validate model first
            if (model == null || (string.IsNullOrEmpty(model.Token) && !Request.Cookies.ContainsKey("refreshToken")))
                return BadRequest(new { message = "Token is required" });

            // Accept token from body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            // Log the received token (Using for debugging)
            //_logger.LogInformation("received token: {Token}", token);

            if(!IsTokenOwner(token))
                return Unauthorized(new { message = "Unauthorized" });

            await _accountService.RevokeTokenAsync(token, ipAddress());
            return Ok(new { message = "Token revoked." });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(IRegisterRequest model)
        {
            await _accountService.RegisterAsync(model, Request.Headers["origin"]);
            return Ok(new { message = "Registration successful, please check your email for verification instructions." });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerfiyEmailAsync(IVerifyEmailRequest model)
        {
            await _accountService.VerifyEmailAsync(model.Token);
            return Ok(new { message = "Verification successful, you can now login." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync(IForgotPasswordRequest model)
        {
            await _accountService.ForgotPasswordAsync(model, Request.Headers["origin"]);
            return Ok(new { message = "Please check your meail for password reset instructions." });
        }

        [HttpPost("validate-reset-token")]
        public async Task<IActionResult> ValidateResetTokenAsync(IValidateResetTokenRequest model)
        {
            await _accountService.ValidateResetTokenAsync(model);
            return Ok(new { message = "Token is valid." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(IResetPasswordRequest model)
        {
            await _accountService.ResetPasswordAsync(model);
            return Ok(new { message = "Password reset successful, you can now login." });
        }

        /// //////////////////
        /// Helper Methods ///
        /// //////////////////

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                //Expires = DateTime.UtcNow.AddSeconds(15),
                SameSite = SameSiteMode.None, // Set SameSite attribute to "None"
                Secure = true // Set the Secure attribute to true for secure (HTTPS) contexts
            };

            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        private bool IsAccountId(int id)
        {
            if (CurrentAccount is Account accountV2)
                return id == accountV2.Id;

            return false;
        }

        private bool IsTokenOwner(string token)
        {
            if (CurrentAccount is Account account)
                return account.OwnsToken(token);

            return false;
        }
    }
}
