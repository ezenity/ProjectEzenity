using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Ezenity.Core.Entities.EmailTemplates;
using Ezenity.Core.Services.Emails;
using Ezenity.DTOs.Models.EmailTemplates;
using Ezenity.DTOs.Models;
using Ezenity.Core.Services.Common;
using Ezenity.Infrastructure.Attributes;
using Ezenity.Core.Helpers.Exceptions;
using System.Text.Json;
using Asp.Versioning;

namespace Ezenity.API.Controllers
{
    /// <summary>
    /// Controller for managing email templates. Inherits from the BaseController class to provide CRUD operations.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/emailtemplates")]
    [ApiVersion("1.0")]
    [Produces("application/vnd.api+json", "application/json", "application/xml")]
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
            _emailTemplateService = emailTemplateService ?? throw new ArgumentNullException(nameof(emailTemplateService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Asynchronously creates a new email template.
        /// </summary>
        /// <param name="model">The email template creation model.</param>
        /// <returns>An action result containing the created email template.</returns>
        [RequestHeaderMatchesMediaType(
            "Accept",
            "application/Ezenity.api.createemailtemplate+json")]
        [Consumes("application/Ezenity.api.createemailtemplate+json")]
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
            "application/Ezenity.api.createemailtemplatetest+json")]
        [Consumes("application/Ezenity.api.createemailtemplatetest+json")]
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
        /// <returns>An action result containing information about the deletion.</returns>
        /// <exception cref="ResourceNotFoundException">Thrown when the email template is not found.</exception>
        /// <exception cref="AuthorizationException">Thrown when the user is unauthorized to perform this action.</exception>
        /// <exception cref="DeletionFailedException">Thrown when the deletion operation fails.</exception>
        /// <exception cref="Exception">Thrown when an unexpected error occurs.</exception>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public override async Task<ActionResult<ApiResponse<DeleteResponse>>> DeleteAsync(int DeleteEmailTemplateId)
        {
            var deletionResult = await _emailTemplateService.DeleteAsync(DeleteEmailTemplateId);

            return Ok(new ApiResponse<DeleteResponse>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Email Template delet succesfully",
                Data = deletionResult
            });
        }

        const int maxEmailTemplatesPageSize = 20;

        /// <summary>
        /// Asynchronously fetches all email templates.
        /// </summary>
        /// <returns>An action result containing a list of email templates.</returns>
        /// <exception cref="Exception">Thrown when an unexpected error occurs.</exception>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmailTemplateResponse>>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public override async Task<ActionResult<ApiResponse<IEnumerable<EmailTemplateResponse>>>> GetAllAsync([FromQuery(Name = "filteronname")] string? name, string? searchQuery, int pageNumber, int pageSize = 10)
        {
            pageSize = Math.Min(pageSize, maxEmailTemplatesPageSize);

            var emailTemplate = await _emailTemplateService.GetAllAsync();

            //var pagedResult = await _emailTemplateService.GetAllAsync(name);
            //var pagedResult = await _emailTemplateService.GetAllAsync(name, searchQuery);
            //var pagedResult = await _emailTemplateService.GetAllAsync(name, searchQuery, pageNumber, pageSize);
            //var pagedResult = await _emailTemplateService.GetAllAsync(name, searchQuery, pageNumber, pageSize);

            //var emailTemplateData = pagedResult.Data;
            //var paginationMetaData = pagedResult.Pagination;

            // Add pagination metadata to the response headers
            //Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

            return Ok(new ApiResponse<IEnumerable<EmailTemplateResponse>>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Email Templates fetched successfully.",
                Data = emailTemplate
                //Data = emailTemplateData,
                //Pagination = paginationMetaData
            });
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
            "application/Ezenity.api.getemailtemplate+json")]
        [Produces("application/Ezenity.api.getemailtemplate+json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<EmailTemplateResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<EmailTemplateResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [HttpGet("{id:int}", Name = "GetEmailTemplate")]
        public override async Task<ActionResult<ApiResponse<EmailTemplateResponse>>> GetByIdAsync(int id)
        {
            var emailTemplate = await _emailTemplateService.GetByIdAsync(id);
            return Ok(new ApiResponse<EmailTemplateResponse>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Email Template fetched successfully.",
                Data = emailTemplate
            });
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
            var updatedEmailTemplate = await _emailTemplateService.UpdateAsync(id, model);
            return Ok(new ApiResponse<EmailTemplateResponse>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Email Template updated successfully.",
                Data = updatedEmailTemplate
            });
        }
    }
}
