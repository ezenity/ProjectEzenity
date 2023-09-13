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

namespace Ezenity_Backend.Controllers
{
    [ApiController]
    [Route("api/emailtemplates")]
    public class EmailTemplatesController : BaseController<EmailTemplate, EmailTemplateResponse, CreateEmailTemplateRequest, UpdateEmailTemplateRequest, DeleteResponse>
    {
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IAuthService _authService;
        private readonly IAccountService _accountService;
        private readonly ILogger<EmailTemplatesController> _logger;

        public EmailTemplatesController(IEmailTemplateService emailTemplateService, IAuthService authService, IAccountService accountService, ILogger<EmailTemplatesController> logger)
        {
            _emailTemplateService = emailTemplateService;
            _authService = authService;
            _accountService = accountService;
            _logger = logger;
        }

        public override async Task<ActionResult<ApiResponse<EmailTemplateResponse>>> CreateAsync(CreateEmailTemplateRequest model)
        {
            var emailTemplate = await _emailTemplateService.CreateAsync(model);
            return Ok(emailTemplate);
        }

        [HttpPost("create-test")]
        public async Task<ActionResult<ApiResponse<EmailTemplateResponse>>> CreateAsyncTest([FromServices]CreateEmailTemplateRequest model)
        {
            var emailTemplate = await _emailTemplateService.CreateAsync(model);
            return Ok(emailTemplate);
        }

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
