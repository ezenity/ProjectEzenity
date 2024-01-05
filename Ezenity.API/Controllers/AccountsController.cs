using Ezenity.API.Controllers;
using Ezenity.API.Filters;
using Ezenity.Core.Entities.Accounts;
using Ezenity.Core.Helpers.Exceptions;
using Ezenity.Core.Services.Common;
using Ezenity.DTOs.Models;
using Ezenity.DTOs.Models.Accounts;
using Ezenity.Infrastructure.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ezenity.API.API.Controllers
{
    /// <summary>
    /// Handles HTTP requests and responses related to Account entities.
    /// </summary>
    [ApiController]
    /*[Route("api/v{version:apiVersion}/accounts")]*/
    [Route("api/accounts")]
    [ApiVersion("1.0")]
    [Produces("application/vnd.api+json")]
    public class AccountsController : BaseController<Account, AccountResponse, CreateAccountRequest, UpdateAccountRequest, DeleteResponse>
    {
        /// <summary>
        /// The service containing business logic for Account entities.
        /// </summary>
        private readonly IAccountService _accountService;

        /// <summary>
        /// Provides logging capabilities for this class.
        /// </summary>
        private readonly ILogger<AccountsController> _logger;

        /// <summary>
        /// Gets the current authenticated account.
        /// </summary>
        /// <value>The current authenticated account.</value>
        public Account CurrentAccount => (Account)HttpContext.Items["Account"];

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountsController"/> class.
        /// </summary>
        /// <param name="accountService">The service to use for account-related operations.</param>
        /// <param name="logger">The logger used for logging any events in this class.</param>
        public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /*
         * Common CRUD methods
         */

        /// <summary>
        /// Fetches an account by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the account to fetch.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing an <see cref="ApiResponse{AccountResponse}"/>. 
        /// The ApiResponse contains the status code, success flag, and a message indicating the result of the operation.
        /// </returns>
        /// <response code="200">Returns the account details if found.</response>
        /// <response code="404">Returns a not found response if the account does not exist.</response>
        /// <response code="500">Returns if an internal server error occurs.</response>
        /// <exception cref="Exception">Thrown when an unexpected error occurs.</exception>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public override async Task<ActionResult<ApiResponse<AccountResponse>>> GetByIdAsync(int id)
        {
            var account = await _accountService.GetByIdAsync(id);
            return Ok(new ApiResponse<AccountResponse>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Account fetched successfully",
                Data = account
            });
        }

        const int maxAccountsPageSize = 20;

        /// <summary>
        /// Fetches all accounts from the database asynchronously.
        /// </summary>
        /// <remarks>
        /// This method fetches all accounts available in the database and returns them as a list. 
        /// It is protected by an "Admin" authorization, meaning only admin users can access it.
        /// </remarks>
        /// <returns>
        /// An <see cref="ActionResult"/> containing an <see cref="ApiResponse{IEnumerable{AccountResponse}}"/>.
        /// The ApiResponse contains the status code, success flag, and a message indicating the result of the operation.
        /// </returns>
        /// <response code="200">Returns a list of all accounts if found.</response>
        /// <response code="404">Returns a not found response if no accounts exist.</response>
        /// <response code="500">Returns if an internal server error occurs.</response>
        /// <exception cref="Exception">Thrown when an unexpected error occurs.</exception>
        //[Authorize("Admin")]
        [Authorize(Policy = "RequireAdminRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public override async Task<ActionResult<ApiResponse<IEnumerable<AccountResponse>>>> GetAllAsync([FromQuery(Name = "filteronname")] string? name, string? searchQuery, int pageNumber, int pageSize = 10)
        {
            pageSize = Math.Min(pageSize, maxAccountsPageSize);

            //var accounts = await _accountService.GetAllAsync();

            //var pagedResult = await _accountService.GetAllAsync(name);
            //var pagedResult = await _accountService.GetAllAsync(name, searchQuery);
            //var pagedResult = await _accountService.GetAllAsync(name, searchQuery, pageNumber, pageSize);
            var pagedResult = await _accountService.GetAllAsync(name, searchQuery, pageNumber, pageSize);

            var accountData = pagedResult.Data;
            var paginationMetaData = pagedResult.Pagination;

            // Add pagination metadata to the response headers
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

            return Ok(new ApiResponse<IEnumerable<AccountResponse>>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Accounts fetched successfully",
                Data = accountData,
                Pagination = paginationMetaData
            });
        }

        /// <summary>
        /// Creates a new account asynchronously.
        /// </summary>
        /// <param name="model">The <see cref="CreateAccountRequest"/> model containing the account details to be created.</param>
        /// <remarks>
        /// This method creates a new account based on the provided CreateAccountRequest model.
        /// The method is protected by "Admin" authorization, meaning only admin users can access it.
        /// If the account with the given email already exists, a conflict response is returned.
        /// </remarks>
        /// <returns>
        /// An <see cref="ActionResult"/> containing an <see cref="ApiResponse{AccountResponse}"/>.
        /// The ApiResponse contains the status code, success flag, and a message indicating the result of the operation.
        /// </returns>
        /// <response code="201">Returns a created response if the account is successfully created.</response>
        /// <response code="404">Returns a not found response if the specified role is not found and/or available.</response>
        /// <response code="409">Returns a conflict response if an account with the given email already exists.</response>
        /// <response code="422">Returns a unprocessable entity response if a role is proivded as a null.</response>
        /// <response code="500">Returns if an internal server error occurs.</response>
        /// <exception cref="Exception">Thrown when an unexpected error occurs.</exception>
        [AuthorizeV2("Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public override async Task<ActionResult<ApiResponse<AccountResponse>>> CreateAsync(CreateAccountRequest model)
        {
            var account = await _accountService.CreateAsync(model);

            // Using the URL Helper to generate the location header.
            var locationUri = Url.Action("GetByIdAsync", new { id = account.Id });

            return Created(locationUri, new ApiResponse<AccountResponse>
            {
                StatusCode = 201,
                IsSuccess = true,
                Message = "Account created successfully",
                Data = account
            });
        }

        /// <summary>
        /// Asynchronously updates an existing account.
        /// </summary>
        /// <param name="id">The identifier of the account to be updated.</param>
        /// <param name="model">The <see cref="UpdateAccountRequest"/> model containing the new account details.</param>
        /// <remarks>
        /// This method updates an existing account based on the provided identifier and UpdateAccountRequest model.
        /// It is authorized for use by any authenticated user.
        /// Multiple error conditions are handled, such as resource not found, authorization failure, or conflict with existing resources.
        /// </remarks>
        /// <returns>
        /// An <see cref="ActionResult"/> containing an <see cref="ApiResponse{AccountResponse}"/>.
        /// The ApiResponse contains the status code, success flag, the updated account data, and a message indicating the result of the operation.
        /// </returns>
        /// <response code="200">Returns an OK response if the account is successfully updated.</response>
        /// <response code="404">Returns a not found response if the account with the specified ID does not exist.</response>
        /// <response code="401">Returns an unauthorized response if the caller lacks necessary permissions.</response>
        /// <response code="409">Returns a conflict response if updating the account would cause a conflict.</response>
        /// <response code="500">Returns an internal server error if an unexpected error occurs.</response>
        /// <exception cref="ResourceNotFoundException">Thrown when the account with the specified ID does not exist.</exception>
        /// <exception cref="AuthorizationException">Thrown when the caller lacks necessary permissions.</exception>
        /// <exception cref="ResourceAlreadyExistsException">Thrown if updating the account would cause a conflict.</exception>
        /// <exception cref="Exception">Thrown when an unexpected error occurs.</exception>
        [AuthorizeV2]
        [HttpPatch(Name = "UpdateAccount")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/vnd.api+json", // JSON:API media type - Default
            "application/json", // Standard JSON media type, if you want to support it
            "application/Ezenity.api.updateaccount+json")] // Custom media type for this specific action
        [Consumes(
            "application/vnd.api+json", // JSON:API media type
            "application/json", // Standard JSON media type
            "application/Ezenity.api.updateaccount+json")] // Custom media type
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status503ServiceUnavailable)]
        [ProducesDefaultResponseType]
        public override async Task<ActionResult<ApiResponse<AccountResponse>>> UpdateAsync(int id, UpdateAccountRequest model)
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


        /*/// <summary>
        /// Asynchronously updates partially on an existing account.
        /// </summary>
        /// <param name="id">The identifier of the account to be updated partially.</param>
        /// <param name="model">The <see cref="UpdateAccountRequest"/> model containing the new account details.</param>
        /// <remarks>
        /// This method partially updates an existing account based on the provided identifier and UpdateAccountRequest model.    
        ///     
        /// This request updates the account's **First Name**:    
        ///     
        ///     PATCH /api/accounts/{id} \
        ///     { \
        ///         "Title": "Mr.", \
        ///         "FirstName": "John", \
        ///         "LastName": "Doe" \
        ///     } \
        ///      
        /// **Note**: All fields are optional and can be patched individually. The update is partial; fields not provided will not be updated.    
        ///     
        /// It is authorized for use by any authenticated user who are the Role Admin.    
        /// Multiple error conditions are handled, such as resource not found, authorization failure, or conflict with existing resources.
        /// </remarks>
        /// <returns>
        /// An <see cref="ActionResult"/> containing an <see cref="ApiResponse{AccountResponse}"/>.
        /// The ApiResponse contains the status code, success flag, the updated account data, and a message indicating the result of the operation.
        /// </returns>
        /// <response code="200">Returns an OK response if the account is successfully updated.</response>
        /// <response code="404">Returns a not found response if the account with the specified ID does not exist.</response>
        /// <response code="401">Returns an unauthorized response if the caller lacks necessary permissions.</response>
        /// <response code="409">Returns a conflict response if updating the account would cause a conflict.</response>
        /// <response code="500">Returns an internal server error if an unexpected error occurs.</response>
        /// <exception cref="ResourceNotFoundException">Thrown when the account with the specified ID does not exist.</exception>
        /// <exception cref="AuthorizationException">Thrown when the caller lacks necessary permissions.</exception>
        /// <exception cref="ResourceAlreadyExistsException">Thrown if updating the account would cause a conflict.</exception>
        /// <exception cref="Exception">Thrown when an unexpected error occurs.</exception>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPatch("{id:int}")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/vnd.api+json", // JSON:API media type - Default
            "application/json", // Standard JSON media type, if you want to support it
            "application/Ezenity.api.updatepartialaccount+json")] // Custom media type for this specific action
        [Consumes(
            "application/vnd.api+json", // JSON:API media type
            "application/json", // Standard JSON media type
            "application/Ezenity.api.updatepartialaccount+json")] // Custom media type
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ApiResponse<AccountResponse>>> UpdatePartialAsync(int id, JsonPatchDocument<UpdateAccountRequest> model)
        {
            try
            {
                //var account = await _accountService.UpdateAsync(id, model);

                return Ok(new ApiResponse<AccountResponse>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    //Data = account,
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
        }*/

        /// <summary>
        /// Asynchronously deletes an account.
        /// </summary>
        /// <param name="DeleteAccountId">The identifier of the account to be deleted.</param>
        /// <remarks>
        /// This method deletes an existing account with the given identifier (DeleteAccountId) and logs who performed the deletion (DeletedById).
        /// The function is authorized for use by any authenticated user.
        /// Multiple error conditions are handled, such as resource not found, authorization failure, or deletion failure.
        /// </remarks>
        /// <returns>
        /// An <see cref="ActionResult"/> containing an <see cref="ApiResponse{DeleteResponse}"/>.
        /// The ApiResponse contains the status code, success flag, a message indicating the result of the operation, and a DeleteResponse object.
        /// </returns>
        /// <response code="200">Returns an OK response if the account is successfully deleted.</response>
        /// <response code="404">Returns a not found response if the account with the specified ID does not exist.</response>
        /// <response code="401">Returns an unauthorized response if the caller lacks necessary permissions.</response>
        /// <response code="400">Returns a bad request response if the deletion fails for some specific reason.</response>
        /// <response code="500">Returns an internal server error if an unexpected error occurs.</response>
        /// <exception cref="ResourceNotFoundException">Thrown when the account with the specified ID does not exist.</exception>
        /// <exception cref="AuthorizationException">Thrown when the caller lacks necessary permissions.</exception>
        /// <exception cref="DeletionFailedException">Thrown if the deletion fails for some specific reason.</exception>
        /// <exception cref="Exception">Thrown when an unexpected error occurs.</exception>
        [AuthorizeV2]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public override async Task<ActionResult<ApiResponse<DeleteResponse>>> DeleteAsync(int DeleteAccountId)
        {
            var deletedAccountData = await _accountService.DeleteAsync(DeleteAccountId);

            return Ok(new ApiResponse<DeleteResponse>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Account deleted successfully",
                Data = deletedAccountData
            });
        }

        /*
         * Uncommon Methods
         */

        /// <summary>
        /// Asynchronously authenticates a user account.
        /// </summary>
        /// <param name="model">The authentication request model containing the credentials.</param>
        /// <remarks>
        /// This endpoint is used to authenticate a user based on the provided credentials.
        /// If authentication is successful, a refresh token is set in a cookie.
        /// The method handles various conditions like account not found or any unexpected server errors.
        /// The endpoint is accessible via POST to "authenticate".
        /// </remarks>
        /// <returns>
        /// An <see cref="ActionResult"/> containing an <see cref="ApiResponse{AuthenticateResponse}"/>.
        /// The ApiResponse contains the status code, success flag, a message indicating the result of the operation, and an AuthenticateResponse object.
        /// </returns>
        /// <response code="200">Returns an OK response if authentication is successful, along with the authentication response data.</response>
        /// <response code="404">Returns a not found response if the account associated with the credentials does not exist.</response>
        /// <response code="500">Returns an internal server error if an unexpected error occurs.</response>
        /// <exception cref="ResourceNotFoundException">Thrown when the account associated with the provided credentials does not exist.</exception>
        /// <exception cref="Exception">Thrown when an unexpected error occurs.</exception>
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthenticateResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<AuthenticateResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<AuthenticateResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ApiResponse<AuthenticateResponse>>> AuthenticateAsync(AuthenticateRequest model)
        {
            var response = await _accountService.AuthenticateAsync(model, ipAddress());
            setTokenCookie(response.RefreshToken);

            var apiResponse = new ApiResponse<AuthenticateResponse>
            {
                StatusCode = 200,
                Message = "Authentication successful.",
                IsSuccess = true,
                Data = response
            };

            return Ok(apiResponse);
        }

        /// <summary>
        /// Asynchronously refreshes the JWT token.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessed via POST to "refresh-token".
        /// The method attempts to fetch the refresh token from the cookie, and if it exists,
        /// it interacts with the account service to obtain a new JWT token.
        /// </remarks>
        /// <returns>
        /// An <see cref="ActionResult"/> containing an <see cref="ApiResponse{AuthenticateResponse}"/>.
        /// The ApiResponse will have status code, success flag, and a message indicating the result of the operation, 
        /// along with an AuthenticateResponse object.
        /// </returns>
        /// <response code="200">Returns OK if the token is successfully refreshed, along with the authentication response data.</response>
        /// <response code="400">Returns BadRequest if the refresh token is missing or invalid.</response>
        /// <response code="500">Returns Internal Server Error if any unexpected error occurs.</response>
        /// <exception cref="ResourceNotFoundException">Thrown when the refresh token is missing or invalid.</exception>
        /// <exception cref="Exception">Thrown when any unexpected error occurs.</exception>
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthenticateResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<AuthenticateResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ApiResponse<AuthenticateResponse>>> RefreshTokenAsync()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                throw new ResourceNotFoundException("Refresh token is missing.");
            var response = await _accountService.RefreshTokenAsync(refreshToken, ipAddress());
            setTokenCookie(response.RefreshToken);

            var apiResponse = new ApiResponse<AuthenticateResponse>
            {
                StatusCode = 200,
                Message = "Token refreshed successfully.",
                IsSuccess = true,
                Data = response
            };

            // Sanitize the IP address by removing new lines and non-numeric or common characters
            string sanitizedIp = System.Text.RegularExpressions.Regex.Replace(ipAddress(), "[^0-9.]", "");

            _logger.LogInformation($"Token refreshed successfully for IP: {sanitizedIp}");

            return Ok(apiResponse);

        }

        /// <summary>
        /// Asynchronously revokes a JWT token.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessed via POST to "revoke-token" and is authorized only for Admin roles.
        /// A service filter for loading the account is applied before executing this method.
        /// The method first validates the input model for the presence of the token either in the model or in cookies.
        /// The token is then checked against the loaded account to ensure it is owned by the same account.
        /// Finally, the token is revoked using the account service.
        /// </remarks>
        /// <param name="model">The <see cref="RevokeTokenRequest"/> containing the token to be revoked.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> that indicates the result of the token revocation.
        /// </returns>
        /// <response code="200">Returns OK with a message indicating that the token has been successfully revoked.</response>
        /// <response code="400">Returns BadRequest if the token is not provided.</response>
        /// <response code="401">Returns Unauthorized if the token is not owned by the loaded account.</response>
        /// <exception cref="Exception">Thrown when any unexpected error occurs.</exception>
        [AuthorizeV2("Admin")]
        [ServiceFilter(typeof(LoadAccountFilter))]
        [HttpPost("revoke-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(RevokeTokenRequest))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
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

        /// <summary>
        /// Asynchronously registers a new account.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessed via POST to "register".
        /// The method attempts to register a new account using the provided model and origin header.
        /// If the registration is successful, a 200 OK status code is returned with a success message.
        /// </remarks>
        /// <param name="model">The <see cref="RegisterRequest"/> containing the account registration details.</param>
        /// <returns>An <see cref="IActionResult"/> that indicates the result of the registration.</returns>
        /// <response code="200">Returns OK with a message indicating successful registration.</response>
        /// <response code="409">Returns Conflict if the account already exists.</response>
        /// <response code="400">Returns BadRequest if the request parameters are not valid.</response>
        /// <response code="500">Returns a StatusCode of 500 if an unexpected error occurs.</response>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> RegisterAsync(RegisterRequest model)
        {
            await _accountService.RegisterAsync(model, Request.Headers["origin"]);

            var apiResponse = new ApiResponse<object>
            {
                StatusCode = 200,
                Message = "Registration successful, please check your email for verification instructions.",
                IsSuccess = true,
                //Data = response
            };

            return Ok(apiResponse);
        }

        /// <summary>
        /// Asynchronously verifies the email address of an account.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessed via POST to "verify-email".
        /// The method attempts to verify the email address using a token provided in the request model.
        /// If the verification is successful, a 200 OK status code is returned with a success message.
        /// </remarks>
        /// <param name="model">The <see cref="VerifyEmailRequest"/> containing the email verification token.</param>
        /// <returns>An <see cref="IActionResult"/> that indicates the result of the email verification.</returns>
        /// <response code="200">Returns OK with a message indicating successful email verification.</response>
        /// <response code="500">Returns a StatusCode of 500 if the token is invalid or an unexpected error occurs.</response>
        [HttpPost("verify-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> VerfiyEmailAsync(VerifyEmailRequest model)
        {
            await _accountService.VerifyEmailAsync(model.Token);

            var apiResponse = new ApiResponse<object>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Verification successful, you can now login."
            };

            return Ok(apiResponse);
        }

        /// <summary>
        /// Initiates the password reset process by sending a password reset token to the user's email.
        /// </summary>
        /// <param name="model">A <see cref="ForgotPasswordRequest"/> object containing the email of the account to reset.</param>
        /// <returns>A message indicating that a password reset instruction has been sent to the user's email.</returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest model)
        {
            await _accountService.ForgotPasswordAsync(model, Request.Headers["origin"]);
            return Ok(new { message = "Please check your meail for password reset instructions." });
        }

        /// <summary>
        /// Validates the provided password reset token.
        /// </summary>
        /// <param name="model">A <see cref="ValidateResetTokenRequest"/> object containing the reset token to be validated.</param>
        /// <returns>A message indicating the validity of the provided reset token.</returns>
        [HttpPost("validate-reset-token")]
        public async Task<IActionResult> ValidateResetTokenAsync(ValidateResetTokenRequest model)
        {
            await _accountService.ValidateResetTokenAsync(model);
            return Ok(new { message = "Token is valid." });
        }

        /// <summary>
        /// Resets the account's password using the provided reset token and new password.
        /// </summary>
        /// <param name="model">A <see cref="ResetPasswordRequest"/> object containing the reset token and new password.</param>
        /// <returns>A message indicating the password has been reset and the user can now login.</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest model)
        {
            await _accountService.ResetPasswordAsync(model);
            return Ok(new { message = "Password reset successful, you can now login." });
        }

        /// //////////////////
        /// Helper Methods ///
        /// //////////////////

        /// <summary>
        /// Sets a cookie that holds the provided token, with options for security and expiration.
        /// </summary>
        /// <param name="token">The token to be stored in the cookie.</param>
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

        /// <summary>
        /// Retrieves the client's IP address from the HTTP request headers or connection information.
        /// </summary>
        /// <returns>The client's IP address as a string.</returns>
        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        /// <summary>
        /// Checks if the current account owns the provided token.
        /// </summary>
        /// <param name="token">The token to be checked for ownership.</param>
        /// <returns>A boolean value indicating whether the current account owns the token.</returns>
        private bool IsTokenOwner(string token)
        {
            if (CurrentAccount is Account account)
                return account.OwnsToken(token);

            return false;
        }
    }
}
