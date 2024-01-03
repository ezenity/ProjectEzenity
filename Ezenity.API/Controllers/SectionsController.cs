using Microsoft.AspNetCore.Mvc;
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
            var section = await _sectionService.GetByIdAsync(id);

            return Ok(new ApiResponse<SectionResponse>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Section fetched successfully.",
                Data = section
            });
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
            pageSize = Math.Min(pageSize, maxSectionsPageSize);

            var sections = await _sectionService.GetAllAsync();

            //var pagedResult = await _emailTemplateService.GetAllAsync(name);
            //var pagedResult = await _emailTemplateService.GetAllAsync(name, searchQuery);
            //var pagedResult = await _emailTemplateService.GetAllAsync(name, searchQuery, pageNumber, pageSize);
            //var pagedResult = await _emailTemplateService.GetAllAsync(name, searchQuery, pageNumber, pageSize);

            //var sectionsData = pagedResult.Data;
            //var paginationMetaData = pagedResult.Pagination;

            // Add pagination metadata to the response headers
            //Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

            return Ok(new ApiResponse<IEnumerable<SectionResponse>>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Sections fetched successfully.",
                Data = sections
                //Data = sectionsData,
                //Pagination = paginationMetaData
            });
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
            var section = await _sectionService.CreateAsync(model);

            return Ok(new ApiResponse<SectionResponse>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Section created successfully.",
                Data = section
            });
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
            var section = await _sectionService.CreateWithAdditionalAsync(model);

            return Ok(new ApiResponse<CreateSectionWithAdditonalRequest>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Section created successfully.",
                Data = section
            });
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
            var updatedSection = await _sectionService.UpdateAsync(id, model);

            return Ok(new ApiResponse<SectionResponse>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Section updated successfully.",
                Data = updatedSection
            });
        }

        /// <summary>
        /// Deletes an existing section based on the provided ID and the ID of the user performing the deletion.
        /// </summary>
        /// <param name="DeleteSectionId">The ID of the section to delete.</param>
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
        public override async Task<ActionResult<ApiResponse<DeleteResponse>>> DeleteAsync(int DeleteSectionId)
        {
            var deletionResult = await _sectionService.DeleteAsync(DeleteSectionId);


            return Ok(new ApiResponse<DeleteResponse>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Account deleted successfully",
                Data = deletionResult
            });
        }
    }
}
