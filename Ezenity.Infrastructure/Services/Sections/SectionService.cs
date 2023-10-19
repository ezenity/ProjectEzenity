using AutoMapper;
using Ezenity.Core.Entities.Sections;
using Ezenity.Core.Helpers.Exceptions;
using Ezenity.Core.Interfaces;
using Ezenity.Core.Services.Common;
using Ezenity.DTOs.Models;
using Ezenity.DTOs.Models.Pages;
using Ezenity.DTOs.Models.Sections;
using Ezenity.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ezenity.Infrastructure.Services.Sections
{
    /// <summary>
    /// Service to handle CRUD operations for sections.
    /// </summary>
    public class SectionService : ISectionService
    {
        /// <summary>
        /// Provides data access to the application's data store.
        /// </summary>
        private readonly IDataContext _context;

        /// <summary>
        /// Provides object-object mapping functionality.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Provides access to application settings.
        /// </summary>
        private readonly IAppSettings _appSettings;

        /// <summary>
        /// Provides logging capabilities.
        /// </summary>
        private readonly ILogger<ISectionService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionService"/> class.
        /// </summary>
        /// <param name="context">Data context for database interaction.</param>
        /// <param name="mapper">Object mapper for model transformation.</param>
        /// <param name="appSettings">Application settings.</param>
        /// <param name="logger">Logger instance.</param>
        public SectionService(IDataContext context, IMapper mapper, IAppSettings appSettings, ILogger<ISectionService> logger)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
            _appSettings = appSettings ?? throw new ArgumentException(nameof(appSettings));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }

        /// <summary>
        /// Retrieves a section by its identifier.
        /// </summary>
        /// <param name="id">Section identifier.</param>
        /// <returns>The section details.</returns>
        public async Task<SectionResponse> GetByIdAsync(int id)
        {
            var section = await GetSection(id);
            return _mapper.Map<SectionResponse>(section);
        }

        /// <summary>
        /// Creates a new section.
        /// </summary>
        /// <param name="model">Data for creating the section.</param>
        /// <returns>The newly created section.</returns>
        public async Task<SectionResponse> CreateAsync(CreateSectionRequest model)
        {
            // Validate
            if (await _context.Sections.AnyAsync(x => x.Title == model.Title))
                throw new AppException($"The Section Title, '{model.Title}', already exist. Please try a different title.");

            /*var section = new Section
            {
                Title = model.Title,
                Layout = model.Layout,
                ContentType = model.ContentType,
                Content = sectionContent
            };*/

            // Map Model to new section object
            var section = _mapper.Map<Section>(model);

            section.Created = DateTime.UtcNow;
            //     skill.Verified = DateTime.UtcNow; // Might not be needed

            // Save the section to the database
            _context.Sections.Add(section);
            await _context.SaveChangesAsync();

            return _mapper.Map<SectionResponse>(section);
        }

        /// <summary>
        /// Asynchronously creates a new section with additional information.
        /// </summary>
        /// <param name="model">The request model containing the details for the new section.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created section details.</returns>
        /// <exception cref="AppException">Thrown when the section title already exists.</exception>
        public async Task<CreateSectionWithAdditonalRequest> CreateWithAdditionalAsync(CreateSectionWithAdditonalRequest model)
        {
            // Validate
            if (await _context.Sections.AnyAsync(x => x.Title == model.Title))
                throw new AppException($"The Section Title, '{model.Title}', already exist. Please try a different title.");

            /*var section = new Section
            {
                Title = model.Title,
                Layout = model.Layout,
                ContentType = model.ContentType,
                Content = sectionContent
            };*/

            // Map Model to new section object
            var section = _mapper.Map<Section>(model);

            section.Created = DateTime.UtcNow;
            //     skill.Verified = DateTime.UtcNow; // Might not be needed

            // Save the section to the database
            _context.Sections.Add(section);
            await _context.SaveChangesAsync();

            return _mapper.Map<CreateSectionWithAdditonalRequest>(section);
        }

        /// <summary>
        /// Deletes a section by its identifier.
        /// </summary>
        /// <param name="id">Section identifier.</param>
        /// <returns>Response after deletion.</returns>
        public async Task<DeleteResponse> DeleteAsync(int id)
        {
            var section = await GetSection(id);
            _context.Sections.Remove(section);
            await _context.SaveChangesAsync();

            return _mapper.Map<DeleteResponse>(section);
        }

        /// <summary>
        /// Retrieves all available sections.
        /// </summary>
        /// <returns>List of all sections.</returns>
        public async Task<IEnumerable<SectionResponse>> GetAllAsync()
        {
            var sections = await _context.Sections.ToListAsync();
            return _mapper.Map<IList<SectionResponse>>(sections);
        }


        public async Task<PagedResult<SectionResponse>> GetAllAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates an existing section.
        /// </summary>
        /// <param name="id">Section identifier.</param>
        /// <param name="model">Data for updating the section.</param>
        /// <returns>The updated section.</returns>
        public async Task<SectionResponse> UpdateAsync(int id, UpdateSectionRequest model)
        {
            var section = await GetSection(id);

            // Validate
            if (section.Title != model.Title && _context.Sections.Any(x => x.Title == model.Title))
                throw new AppException($"The Section Title, '{model.Title}', already exist, please try a different title.");

            // Update the common properties
            _mapper.Map(model, section);

            section.Updated = DateTime.UtcNow;

            _context.Sections.Update(section);
            await _context.SaveChangesAsync();

            return _mapper.Map<SectionResponse>(section);
        }

        /// <summary>
        /// Helper method to fetch a section by its identifier.
        /// </summary>
        /// <param name="id">Section identifier.</param>
        /// <returns>The section entity.</returns>
        private async Task<Section> GetSection(int id)
        {
            /*var section = _context.Sections.Include(s => s.Content).SingleOrDefault(s => s.Id == id);*/
            var section = await _context.Sections.FindAsync(id);
            if (section == null)
                throw new KeyNotFoundException("Section not found");
            return section;
        }
    }
}
