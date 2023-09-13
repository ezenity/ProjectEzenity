using Ezenity_Backend.Entities.Sections;
using Ezenity_Backend.Helpers;
using Ezenity_Backend.Helpers.Exceptions;
using Ezenity_Backend.Models;
using Ezenity_Backend.Models.Sections;
using Ezenity_Backend.Services.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ezenity_Backend.Controllers
{
    [ApiController]
    [Route("api/sections")]
    public class SectionsController : BaseController<Section, SectionResponse, CreateSectionRequest, UpdateSectionRequest, DeleteResponse>
    {
        private readonly ISectionService _sectionService;
        private readonly IAccountService _accountService;
        private readonly ILogger<SectionsController> _logger;

        public SectionsController(ISectionService sectionService, IAccountService accountService, ILogger<SectionsController> logger)
        {
            _sectionService = sectionService;
            _accountService = accountService;
            _logger = logger;
        }

        public override async Task<ActionResult<ApiResponse<SectionResponse>>> GetByIdAsync(int id)
        {
            ApiResponse<SectionResponse> apiResponse = new ApiResponse<SectionResponse>();

            try
            {
                var section = await _sectionService.GetByIdAsync(id);

                apiResponse.StatusCode = 200;
                apiResponse.Message = "Section fetched successfully.";
                apiResponse.IsSuccess = true;
                apiResponse.Data = section;

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
                _logger.LogError("[Error] Section fetched unsuccessfully: {0}", ex);

                apiResponse.StatusCode = 500;
                apiResponse.Message = "An error occurred while fetching the section.";
                apiResponse.IsSuccess = false;
                apiResponse.Errors.Add(ex.Message);

                return StatusCode(500, apiResponse);
            }
        }

        public override async Task<ActionResult<ApiResponse<IEnumerable<SectionResponse>>>> GetAllAsync()
        {
            //IApiResponse<ISectionResponse> apiResponse = new ApiResponse<ISectionResponse>();
            ApiResponse<IEnumerable<SectionResponse>> apiResponse = new ApiResponse<IEnumerable<SectionResponse>>();


            try
            {
                var sections = await _sectionService.GetAllAsync();

                apiResponse.StatusCode = 200;
                apiResponse.Message = "Sections fetched successfully.";
                apiResponse.IsSuccess = true;
                apiResponse.Data = sections;

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error] Sections fetched unsuccessfully: {0}", ex);

                apiResponse.StatusCode = 500;
                apiResponse.Message = "An error occurred while fetching the sections.";
                apiResponse.IsSuccess = false;
                apiResponse.Errors.Add(ex.Message);

                return StatusCode(500, apiResponse);
            }
        }

        public override async Task<ActionResult<ApiResponse<SectionResponse>>> CreateAsync(CreateSectionRequest model)
        {
            ApiResponse<SectionResponse> apiResponse = new ApiResponse<SectionResponse>();

            try
            {
                var section = await _sectionService.CreateAsync(model);

                apiResponse.StatusCode = 200;
                apiResponse.Message = "Section created successfully.";
                apiResponse.IsSuccess = true;
                apiResponse.Data = section;

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error] Section creation unsuccessfully: {0}", ex);

                apiResponse.StatusCode = 500;
                apiResponse.Message = "An error occurred while creating the sections.";
                apiResponse.IsSuccess = false;
                apiResponse.Errors.Add(ex.Message);

                return StatusCode(500, apiResponse);
            }
        }

        [Authorize("Admin")]
        public override async Task<ActionResult<ApiResponse<SectionResponse>>> UpdateAsync(int id, UpdateSectionRequest model)
        {
            ApiResponse<SectionResponse> apiResponse = new ApiResponse<SectionResponse>();

            try
            {
                var updatedSection = await _sectionService.UpdateAsync(id, model);

                apiResponse.StatusCode = 200;
                apiResponse.Message = "Section updated successfully.";
                apiResponse.IsSuccess = true;
                apiResponse.Data = updatedSection;

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
                _logger.LogError("[Error] Section updated unsuccessfully: {0}", ex);

                apiResponse.StatusCode = 500;
                apiResponse.Message = "An error occurred while updating the section.";
                apiResponse.IsSuccess = false;
                apiResponse.Errors.Add(ex.Message);

                return StatusCode(500, apiResponse);
            }
        }

        [Authorize("Admin")]
        public override async Task<ActionResult<ApiResponse<DeleteResponse>>> DeleteAsync(int DeleteSectionId, int DeletedById)
        {
            DeleteResponse deleteResponse = new DeleteResponse();
            List<string> errors = new List<string>();

            try
            {
                var account = await _accountService.GetByIdAsync(DeletedById);
                var deletionResult = await _sectionService.DeleteAsync(DeleteSectionId);

                deleteResponse.Message = "Section deleted succesfully";
                deleteResponse.StatusCode = 200;
                deleteResponse.DeletedBy = account;
                deleteResponse.DeletedAt = DateTime.UtcNow;
                deleteResponse.ResourceId = DeleteSectionId.ToString();
                deleteResponse.IsSuccess = true;


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
    }
}
