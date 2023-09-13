using Ezenity_Backend.Entities.Accounts;
using Ezenity_Backend.Filters;
using Ezenity_Backend.Helpers;
using Ezenity_Backend.Helpers.Exceptions;
using Ezenity_Backend.Models;
using Ezenity_Backend.Models.Accounts;
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
    public class AccountsController : BaseController<Account, AccountResponse, CreateAccountRequest, UpdateAccountRequest, DeleteResponse>
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountsController> _logger;

        // Property for the current authenticated account
        public Account CurrentAccount => (Account)HttpContext.Items["Account"];

        public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        // Common CRUD methods
        public override async Task<ActionResult<ApiResponse<AccountResponse>>> GetByIdAsync(int id)
        {
            try
            {
                var account = await _accountService.GetByIdAsync(id);

                if (account == null)
                {
                    return NotFound(new ApiResponse<AccountResponse>
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = "Account not found."
                    });
                }

                return Ok(new ApiResponse<AccountResponse>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "Account fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error] Account fetched unsuccessfully: {0}", ex);

                return StatusCode(500, new ApiResponse<AccountResponse>
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = "An error occurred while fetching the Account."
                });
            }
        }

        [Authorize("Admin")]
        public override async Task<ActionResult<ApiResponse<IEnumerable<AccountResponse>>>> GetAllAsync()
        {
            try
            {
                var accounts = await _accountService.GetAllAsync();

                if (accounts == null)
                {
                    return NotFound(new ApiResponse<AccountResponse>
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = "Accounts not found."
                    });
                }

                return Ok(new ApiResponse<AccountResponse>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "Accounts fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error] Accounts fetched unsuccessfully: {0}", ex);

                return StatusCode(500, new ApiResponse<AccountResponse>
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = "An error occurred while fetching the Accounts."
                });
            }
        }

        [Authorize("Admin")]
        public override async Task<ActionResult<ApiResponse<AccountResponse>>> CreateAsync(CreateAccountRequest model)
        {
            try
            {
                var account = await _accountService.CreateAsync(model);

                if (account.Email == model.Email)
                {
                    return Conflict(new ApiResponse<AccountResponse>
                    {
                        StatusCode = 409,
                        IsSuccess = false,
                        Message = "An Account with this email already exists."
                    });
                }

                return Created(""/* TODO: input API endpoint to get account details (URL) */, new ApiResponse<AccountResponse>
                {
                    StatusCode = 201,
                    IsSuccess = true,
                    Message = "Account created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error] Accounts created unsuccessfully: {0}", ex);

                return StatusCode(500, new ApiResponse<AccountResponse>
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = "An error occurred while creating the Account."
                });
            }
        }

        [Authorize]
        public override async Task<ActionResult<ApiResponse<AccountResponse>>> UpdateAsync(int id, UpdateAccountRequest model)
        {
            try
            {
                var account = await _accountService.UpdateAsync(id, model);

                return Ok(new ApiResponse<AccountResponse>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Data = account,
                    Message = "Account created successfully"
                });
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(new ApiResponse<AccountResponse>
                {
                    StatusCode = 404,
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (AuthorizationException ex)
            {
                return Unauthorized(new ApiResponse<AccountResponse>
                {
                    StatusCode = 401,
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (ResourceAlreadyExistsException ex)
            {
                return Conflict(new ApiResponse<AccountResponse>
                {
                    StatusCode = 409,
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error] Accounts created unsuccessfully: {0}", ex);

                return StatusCode(500, new ApiResponse<AccountResponse>
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = "An error occurred while creating the Account."
                });
            }
        }

        [Authorize]
        public override async Task<ActionResult<ApiResponse<DeleteResponse>>> DeleteAsync(int DeleteAccountId, int DeletedById)
        {
            try
            {
                var account = await _accountService.GetByIdAsync(DeletedById);
                var deletionResult = await _accountService.DeleteAsync(DeleteAccountId);

                DeleteResponse deleteResponse = new DeleteResponse
                {
                    Message = "Account deleted successfully",
                    DeletedBy = account,
                    DeletedAt = DateTime.UtcNow
                };

                return Ok(new ApiResponse<DeleteResponse>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "Account deleted successfully",
                    Data = deleteResponse
                });

            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(new ApiResponse<DeleteResponse>
                {
                    StatusCode = 404,
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (AuthorizationException ex)
            {
                return Unauthorized(new ApiResponse<DeleteResponse>
                {
                    StatusCode = 401,
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (DeletionFailedException ex)
            {
                return BadRequest(new ApiResponse<DeleteResponse>
                {
                    StatusCode = 400,
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<DeleteResponse>
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = "An unexpected error occurred"
                });
            }
        }

        // Uncommon Methods

        [HttpPost("authenticate")]
        public async Task<ActionResult<ApiResponse<AuthenticateResponse>>> AuthenticateAsync(AuthenticateRequest model)
        {
            ApiResponse<AuthenticateResponse> apiResponse = new ApiResponse<AuthenticateResponse>();

            try
            {
                var response = await _accountService.AuthenticateAsync(model, ipAddress());

                setTokenCookie(response.RefreshToken);

                apiResponse.StatusCode = 200;
                apiResponse.Message = "Authentication successfull.";
                apiResponse.IsSuccess = true;
                apiResponse.Data = response;

                return Ok(apiResponse);
            }
            catch (ResourceNotFoundException ex)
            {
                apiResponse.StatusCode = 404;
                apiResponse.Message = ex.Message;
                apiResponse.IsSuccess = false;
                apiResponse.Errors.Add(ex.Message);

                return NotFound(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.StatusCode = 500;
                apiResponse.Message = "An unexpected error occurred.";
                apiResponse.IsSuccess = false;
                apiResponse.Errors.Add(ex.Message);

                return StatusCode(500, apiResponse);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<AuthenticateResponse>>> RefreshTokenAsync()
        {
            ApiResponse<AuthenticateResponse> apiResponse = new ApiResponse<AuthenticateResponse>();

            try
            {
                var refreshToken = Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(refreshToken))
                    throw new ResourceNotFoundException("Refresh token is missing.");

                var response = await _accountService.RefreshTokenAsync(refreshToken, ipAddress());

                setTokenCookie(response.RefreshToken);

                apiResponse.StatusCode = 200;
                apiResponse.Message = "Token refreshed successfully.";
                apiResponse.IsSuccess = true;
                apiResponse.Data = response;

                _logger.LogInformation("Token refreshed successfully for IP: {0}", ipAddress());

                return Ok(apiResponse);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError("Failed to refresh token: {0}", ex.Message);

                apiResponse.StatusCode = 400;
                apiResponse.Message = ex.Message;
                apiResponse.IsSuccess = false;
                apiResponse.Errors.Add(ex.Message);

                return BadRequest(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred: {0}", ex.Message);

                apiResponse.StatusCode = 500;
                apiResponse.Message = "An unexpected error occurred.";
                apiResponse.IsSuccess = false;
                apiResponse.Errors.Add(ex.Message);

                return StatusCode(500, apiResponse);
            }

        }

        [Authorize("Admin")]
        [ServiceFilter(typeof(LoadAccountFilter))]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeTokenAsync(RevokeTokenRequest model)
        {
            // Validate model first
            if (model == null || (string.IsNullOrEmpty(model.Token) && !Request.Cookies.ContainsKey("refreshToken")))
                return BadRequest(new { message = "Token is required" });

            // Accept token from body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            // Log the received token (Using for debugging)
            //_logger.LogInformation("received token: {Token}", token);

            //var entity = this.Entity;

            //entity.OwnsToken(token);

            /*if (!IsTokenOwner(token))
                    return Unauthorized(new { message = "Unauthorized" });*/

            // Retrieve the account loaded by the LoadAccountFilter
            var loadedAccount = Entity;

            if(loadedAccount != null || loadedAccount.OwnsToken(token))
                return Unauthorized(new { message = "Unauthorized" });

            await _accountService.RevokeTokenAsync(token, ipAddress());
            return Ok(new { message = "Token revoked." });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest model)
        {
            ApiResponse<object> apiResponse = new ApiResponse<object>();

            try
            {
                await _accountService.RegisterAsync(model, Request.Headers["origin"]);

                apiResponse.StatusCode = 200;
                apiResponse.Message = "Registration successful, please check your email for verification instructions.";
                apiResponse.IsSuccess = true;
                //apiResponse.Data = response;

                return Ok(apiResponse);
            }
            catch (ResourceAlreadyExistsException ex)
            {
                return Conflict(new ApiResponse<object>
                {
                    StatusCode = 409,
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (ResourceNotFoundException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error] Account registered unsuccessfully: {0}", ex);

                return StatusCode(500, new ApiResponse<object>
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = "An error occurred while creating the Account."
                });
            }
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerfiyEmailAsync(VerifyEmailRequest model)
        {
            ApiResponse<object> apiResponse = new ApiResponse<object>();

            try
            {
                await _accountService.VerifyEmailAsync(model.Token);

                apiResponse.StatusCode = 200;
                apiResponse.IsSuccess = true;
                apiResponse.Message = "Verification successful, you can now login.";

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error] Account verification unsuccessfully: {0}", ex);

                apiResponse.StatusCode = 500;
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;

                return StatusCode(500, apiResponse);
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest model)
        {
            await _accountService.ForgotPasswordAsync(model, Request.Headers["origin"]);
            return Ok(new { message = "Please check your meail for password reset instructions." });
        }

        [HttpPost("validate-reset-token")]
        public async Task<IActionResult> ValidateResetTokenAsync(ValidateResetTokenRequest model)
        {
            await _accountService.ValidateResetTokenAsync(model);
            return Ok(new { message = "Token is valid." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest model)
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

        private bool IsTokenOwner(string token)
        {
            if (CurrentAccount is Account account)
                return account.OwnsToken(token);

            return false;
        }
    }
}
