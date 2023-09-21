using Ezenity_Backend.Entities.EmailTemplates;
using Ezenity_Backend.Helpers.Exceptions;
using Ezenity_Backend.Models;
using Ezenity_Backend.Models.EmailTemplates;
using Ezenity_Backend.Services.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Ezenity_Backend.Attributes;

namespace Ezenity_Backend.Controllers
{
    /// <summary>
    /// Controller for managing email templates. Inherits from the BaseController class to provide CRUD operations.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/emailtemplates")]
    [ApiVersion("1.0")]
    [Produces("application/json", "application/xml")]
    public class EmailTemplatesController : BaseController<EmailTemplate, EmailTemplateResponse, CreateEmailTemplateRequest, UpdateEmailTemplateRequest, DeleteResponse>
    {
        /// <summary>
        /// Service responsible for email template related functionalities.
        /// </summary>
        private readonly IEmailTemplateService _emailTemplateService;

        /// <summary>
        /// Service responsible for authentication related functionalities.
        /// </summary>
        private readonly IAuthService _authService;

        /// <summary>
        /// Service responsible for account related functionalities.
        /// </summary>
        private readonly IAccountService _accountService;

        /// <summary>
        /// Logger instance for logging important or error information.
        /// </summary>
        private readonly ILogger<EmailTemplatesController> _logger;

        /// <summary>
        /// Initializes a new instance of the EmailTemplatesController class.
        /// </summary>
        /// <param name="emailTemplateService">The email template service.</param>
        /// <param name="authService">The authentication service.</param>
        /// <param name="accountService">The account service.</param>
        /// <param name="logger">The logger service.</param>
        public EmailTemplatesController(IEmailTemplateService emailTemplateService, IAuthService authService, IAccountService accountService, ILogger<EmailTemplatesController> logger)
        {
            _emailTemplateService = emailTemplateService;
            _authService = authService;
            _accountService = accountService;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously creates a new email template.
        /// </summary>
        /// <param name="model">The email template creation model.</param>
        /// <returns>An action result containing the created email template.</returns>
        [RequestHeaderMatchesMediaType(
            "Accept",
            "application/ezenity.api.createemailtemplate+json")]
        [Consumes("application/ezenity.api.createemailtemplate+json")]
        [HttpPost(Name = "CreateEmailTemplate")]
        public override async Task<ActionResult<ApiResponse<EmailTemplateResponse>>> CreateAsync(CreateEmailTemplateRequest model)
        {
            var emailTemplate = await _emailTemplateService.CreateAsync(model);
            return Ok(emailTemplate);
        }

        /// <summary>
        /// Asynchronously creates a new email template
        ///  - This is a test method which requires no Authentication.
        /// </summary>
        /// <param name="model">The email template creation model.</param>
        /// <returns>An action result containing the created email template.</returns>
        [RequestHeaderMatchesMediaType(
            "Accept",
            "application/ezenity.api.createemailtemplatetest+json")]
        [Consumes("application/ezenity.api.createemailtemplatetest+json")]
        [HttpPost("create-test")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ApiResponse<EmailTemplateResponse>>> CreateAsyncTest([FromServices] CreateEmailTemplateRequest model)
            //public async Task<ActionResult<ApiResponse<EmailTemplateResponse>>> CreateAsyncTest([FromServices] CreateEmailTemplateRequest model)
        {
            var emailTemplate = await _emailTemplateService.CreateAsync(model);
            return Ok(emailTemplate);
        }

        /// <summary>
        /// Asynchronously deletes an existing email template.
        /// </summary>
        /// <param name="DeleteEmailTemplateId">The ID of the email template to delete.</param>
        /// <param name="DeletedById">The ID of the user performing the deletion.</param>
        /// <returns>An action result containing information about the deletion.</returns>
        /// <exception cref="ResourceNotFoundException">Thrown when the email template is not found.</exception>
        /// <exception cref="AuthorizationException">Thrown when the user is unauthorized to perform this action.</exception>
        /// <exception cref="DeletionFailedException">Thrown when the deletion operation fails.</exception>
        /// <exception cref="Exception">Thrown when an unexpected error occurs.</exception>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public override async Task<ActionResult<ApiResponse<DeleteResponse>>> DeleteAsync(int DeleteEmailTemplateId, int DeletedById)
        {
            DeleteResponse deleteResponse = new DeleteResponse();
            List<string> errors = new List<string>();

            try
            {
                var account = await _accountService.GetByIdAsync(DeletedById);
                var deletionResult = await _emailTemplateService.DeleteAsync(DeleteEmailTemplateId);

                deleteResponse.Message = "Email Template delet succesfully";
                deleteResponse.StatusCode = 200;
                deleteResponse.DeletedBy = account;
                deleteResponse.DeletedAt = DateTime.UtcNow;
                deleteResponse.ResourceId = DeleteEmailTemplateId.ToString();
                deleteResponse.IsSuccess = true;

                return Ok(new ApiResponse<DeleteResponse>
                {
                    StatusCode = deleteResponse.StatusCode,
                    IsSuccess = deleteResponse.IsSuccess,
                    Message = deleteResponse.Message,
                    Data = deleteResponse
                });
            }
            catch (ResourceNotFoundException ex)
            {
                errors.Add(ex.Message);
                deleteResponse.Errors = errors;
                deleteResponse.StatusCode = 404;
                deleteResponse.IsSuccess = false;

                return NotFound(new ApiResponse<DeleteResponse>
                {
                    StatusCode = deleteResponse.StatusCode,
                    IsSuccess = deleteResponse.IsSuccess,
                    Message = ex.Message,
                    Data = deleteResponse
                });
            }
            catch (AuthorizationException ex)
            {
                errors.Add(ex.Message);
                deleteResponse.Errors = errors;
                deleteResponse.StatusCode = 401;
                deleteResponse.IsSuccess = false;

                return Unauthorized(new ApiResponse<DeleteResponse>
                {
                    StatusCode = deleteResponse.StatusCode,
                    IsSuccess = deleteResponse.IsSuccess,
                    Message = ex.Message,
                    Data = deleteResponse
                });
            }
            catch (DeletionFailedException ex)
            {
                errors.Add(ex.Message);
                deleteResponse.Errors = errors;
                deleteResponse.StatusCode = 400;
                deleteResponse.IsSuccess = false;

                return BadRequest(new ApiResponse<DeleteResponse>
                {
                    StatusCode = deleteResponse.StatusCode,
                    IsSuccess = deleteResponse.IsSuccess,
                    Message = ex.Message,
                    Data = deleteResponse
                });
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                deleteResponse.Errors = errors;
                deleteResponse.StatusCode = 500;
                deleteResponse.IsSuccess = false;

                return StatusCode(500, new ApiResponse<DeleteResponse>
                {
                    StatusCode = deleteResponse.StatusCode,
                    IsSuccess = deleteResponse.IsSuccess,
                    Message = ex.Message,
                    Data = deleteResponse
                });
            }

        }

        /// <summary>
        /// Asynchronously fetches all email templates.
        /// </summary>
        /// <returns>An action result containing a list of email templates.</returns>
        /// <exception cref="Exception">Thrown when an unexpected error occurs.</exception>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmailTemplateResponse>>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public override async Task<ActionResult<ApiResponse<IEnumerable<EmailTemplateResponse>>>> GetAllAsync()
        {
            ApiResponse<IEnumerable<EmailTemplateResponse>> apiResponse = new ApiResponse<IEnumerable<EmailTemplateResponse>>();

            try
            {
                var emailTemplate = await _emailTemplateService.GetAllAsync();

                apiResponse.StatusCode = 200;
                apiResponse.Message = "Email Templates fetched successfully.";
                apiResponse.IsSuccess = true;
                apiResponse.Data = emailTemplate;

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error] Email Template fetched unsuccessfully: {0}", ex);

                apiResponse.StatusCode = 500;
                apiResponse.Message = "An error occurred while fetching the Email Templates.";
                apiResponse.IsSuccess = false;
                apiResponse.Errors.Add(ex.Message);

                return StatusCode(500, apiResponse);
            }
        }

        /// <summary>
        /// Asynchronously fetches an email template by its ID.
        /// </summary>
        /// <param name="id">The ID of the email template to fetch.</param>
        /// <returns>An action result containing the fetched email template.</returns>
        /// <exception cref="ResourceNotFoundException">Thrown when the email template is not found.</exception>
        /// <exception cref="Exception">Thrown when an unexpected error occurs.</exception>
        [RequestHeaderMatchesMediaType(
            "Accept",
            "application/ezenity.api.getemailtemplate+json")]
        [Produces("application/ezenity.api.getemailtemplate+json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<EmailTemplateResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<EmailTemplateResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [HttpGet("{id:int}", Name = "GetEmailTemplate")]
        public override async Task<ActionResult<ApiResponse<EmailTemplateResponse>>> GetByIdAsync(int id)
        {
            ApiResponse<EmailTemplateResponse> apiResponse = new ApiResponse<EmailTemplateResponse>();

            try
            {
                var emailTemplate = await _emailTemplateService.GetByIdAsync(id);

                apiResponse.StatusCode = 200;
                apiResponse.Message = "Email Template fetched successfully.";
                apiResponse.IsSuccess = true;
                apiResponse.Data = emailTemplate;

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
                _logger.LogError("[Error] Email Template fetched unsuccessfully: {0}", ex);

                apiResponse.StatusCode = 500;
                apiResponse.Message = "An error occurred while fetching the Email Template.";
                apiResponse.IsSuccess = false;
                apiResponse.Errors.Add(ex.Message);

                return StatusCode(500, apiResponse);
            }
        }

        /// <summary>
        /// Retrieves a non-dynamic email template by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the email template.</param>
        /// <returns>An API response containing the requested email template.</returns>
        /// <exception cref="ResourceNotFoundException">Thrown if the email template does not exist.</exception>
        /// <exception cref="Exception">Thrown when an unexpected error occurs.</exception>
        [RequestHeaderMatchesMediaType(
            "Accept",
            "application/ezenity.api.getemailtemplatenondynamiccontent+json")]
        [Produces("application/ezenity.api.getemailtemplatenondynamiccontent+json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<EmailTemplateResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<EmailTemplateResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [HttpGet("{id:int}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ApiResponse<EmailTemplateNonDynamicResponse>>> GetNonDynamicByIdAsync(int id)
        {
            ApiResponse<EmailTemplateNonDynamicResponse> apiResponse = new ApiResponse<EmailTemplateNonDynamicResponse>();

            try
            {
                var emailTemplate = await _emailTemplateService.GetNonDynamicByIdAsync(id);

                apiResponse.StatusCode = 200;
                apiResponse.Message = "Email Template fetched successfully.";
                apiResponse.IsSuccess = true;
                apiResponse.Data = emailTemplate;

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
                _logger.LogError("[Error] Email Template fetched unsuccessfully: {0}", ex);

                apiResponse.StatusCode = 500;
                apiResponse.Message = "An error occurred while fetching the Email Template.";
                apiResponse.IsSuccess = false;
                apiResponse.Errors.Add(ex.Message);

                return StatusCode(500, apiResponse);
            }
        }

        /// <summary>
        /// Asynchronously updates an existing email template.
        /// </summary>
        /// <param name="id">The ID of the email template to update.</param>
        /// <param name="model">The email template update model.</param>
        /// <returns>An action result containing the updated email template.</returns>
        /// <exception cref="ResourceNotFoundException">Thrown when the email template is not found.</exception>
        /// <exception cref="AuthorizationException">Thrown when the user is unauthorized to perform this action.</exception>
        /// <exception cref="Exception">Thrown when an unexpected error occurs.</exception>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<EmailTemplateResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<EmailTemplateResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<EmailTemplateResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public override async Task<ActionResult<ApiResponse<EmailTemplateResponse>>> UpdateAsync(int id, UpdateEmailTemplateRequest model)
        {
            ApiResponse<EmailTemplateResponse> apiResponse = new ApiResponse<EmailTemplateResponse>();

            try
            {
                var updatedEmailTemplate = await _emailTemplateService.UpdateAsync(id, model);

                apiResponse.StatusCode = 200;
                apiResponse.IsSuccess = true;
                apiResponse.Data = updatedEmailTemplate;
                apiResponse.Message = "Email Template updated successfully.";

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
            catch (AuthorizationException ex)
            {
                apiResponse.StatusCode = 401;
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;


                return Unauthorized(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error] Email Template fetched unsuccessfully: {0}", ex);

                apiResponse.StatusCode = 500;
                apiResponse.Message = "An error occurred while fetching the Email Templates.";
                apiResponse.IsSuccess = false;
                apiResponse.Errors.Add(ex.Message);

                return StatusCode(500, apiResponse);
            }
        }
    }
}
