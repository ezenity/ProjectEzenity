﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using static System.Collections.Specialized.BitVector32;
using Ezenity.DTOs.Models.Sections;
using Ezenity.DTOs.Models;
using Ezenity.Core.Services.Common;
using Ezenity.Core.Helpers.Exceptions;
using Ezenity.Infrastructure.Attributes;

namespace Ezenity.API.Controllers
{
    /// <summary>
    /// Provides an API controller for managing sections.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/sections")]
    [ApiVersion("1.0")]
    [Produces("application/json", "application/xml")]
    public class SectionsController : BaseController<Section, SectionResponse, CreateSectionRequest, UpdateSectionRequest, DeleteResponse>
    {
        /// <summary>
        /// Service to handle section-related business logic.
        /// </summary>
        private readonly ISectionService _sectionService;

        /// <summary>
        /// Service to handle account-related business logic.
        /// </summary>
        private readonly IAccountService _accountService;

        /// <summary>
        /// Logger for capturing runtime information.
        /// </summary>
        private readonly ILogger<SectionsController> _logger;

        /// <summary>
        /// Initializes a new instance of the SectionsController class.
        /// </summary>
        /// <param name="sectionService">Service for section-related business logic.</param>
        /// <param name="accountService">Service for account-related business logic.</param>
        /// <param name="logger">Logger instance for capturing runtime logs.</param>
        public SectionsController(ISectionService sectionService, IAccountService accountService, ILogger<SectionsController> logger)
        {
            _sectionService = sectionService ?? throw new ArgumentNullException(nameof(logger));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(logger));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Fetches a section by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the section to fetch.</param>
        /// <returns>A wrapped API response containing the section data or errors.</returns>
        /// <exception cref="ResourceNotFoundException">Thrown when the requested section is not found.</exception>
        /// <exception cref="Exception">Thrown for generic server errors.</exception>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<SectionResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<SectionResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
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

        const int maxSectionsPageSize = 20;

        /// <summary>
        /// Fetches all sections asynchronously.
        /// </summary>
        /// <returns>A wrapped API response containing the list of sections or errors.</returns>
        /// <exception cref="Exception">Thrown for generic server errors.</exception>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SectionResponse>>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public override async Task<ActionResult<ApiResponse<IEnumerable<SectionResponse>>>> GetAllAsync([FromQuery(Name = "filteronname")] string? name, string? searchQuery, int pageNumber, int pageSize = 10)
        {
            //IApiResponse<ISectionResponse> apiResponse = new ApiResponse<ISectionResponse>();
            ApiResponse<IEnumerable<SectionResponse>> apiResponse = new ApiResponse<IEnumerable<SectionResponse>>();

            if(pageSize > maxSectionsPageSize)
                pageSize = maxSectionsPageSize;


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

        /// <summary>
        /// Creates a new section based on the provided request model.
        /// </summary>
        /// <param name="model">The request model containing the data to create a new section.</param>
        /// <returns>A wrapped API response containing the created section or errors.</returns>
        /// <exception cref="Exception">Thrown for generic server errors.</exception>
        [HttpPost(Name = "CreateSection")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/Ezenity.api.createsection+json")]
        [Consumes(
            "application/json",
            "application/Ezenity.api.createsection+json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<SectionResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
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

        /// <summary>
        /// Creates a new section with additional properties asynchronously.
        /// </summary>
        /// <param name="model">The model containing data for the new section.</param>
        /// <returns>An API response containing the created section.</returns>
        /// <exception cref="AppException">Thrown if the section title already exists.</exception>
        [HttpPost]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/Ezenity.api.createsectionwithadditionalproperties+json")]
        [Consumes(
            "application/Ezenity.api.createsectionwithadditionalproperties+json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<SectionResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [ApiExplorerSettings(IgnoreApi = true)] 
        public async Task<ActionResult<ApiResponse<CreateSectionWithAdditonalRequest>>> CreateWithAdditionalAsync(CreateSectionWithAdditonalRequest model)
        {
            ApiResponse<CreateSectionWithAdditonalRequest> apiResponse = new ApiResponse<CreateSectionWithAdditonalRequest>();

            try
            {
                var section = await _sectionService.CreateWithAdditionalAsync(model);

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

        /// <summary>
        /// Updates an existing section based on the provided ID and request model.
        /// </summary>
        /// <param name="id">The ID of the section to update.</param>
        /// <param name="model">The request model containing the data to update the section.</param>
        /// <returns>A wrapped API response containing the updated section or errors.</returns>
        /// <exception cref="ResourceNotFoundException">Thrown when the requested section is not found.</exception>
        /// <exception cref="Exception">Thrown for generic server errors.</exception>
        [AuthorizeV2("Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<SectionResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<SectionResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
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

        /// <summary>
        /// Deletes an existing section based on the provided ID and the ID of the user performing the deletion.
        /// </summary>
        /// <param name="DeleteSectionId">The ID of the section to delete.</param>
        /// <param name="DeletedById">The ID of the user performing the deletion.</param>
        /// <returns>A wrapped API response containing the deletion status or errors.</returns>
        /// <exception cref="ResourceNotFoundException">Thrown when the requested section or user is not found.</exception>
        /// <exception cref="AuthorizationException">Thrown when the user is not authorized to perform the deletion.</exception>
        /// <exception cref="DeletionFailedException">Thrown when deletion fails due to server or validation errors.</exception>
        /// <exception cref="Exception">Thrown for generic server errors.</exception>
        [AuthorizeV2("Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<DeleteResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
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