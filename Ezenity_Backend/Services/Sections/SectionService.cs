using AutoMapper;
using Ezenity_Backend.Entities.Sections;
using Ezenity_Backend.Helpers;
using Ezenity_Backend.Helpers.Exceptions;
using Ezenity_Backend.Models;
using Ezenity_Backend.Models.Sections;
using Ezenity_Backend.Services.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ezenity_Backend.Services.Sections
{
    public class SectionService : ISectionService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly ILogger<ISectionService> _logger;

        public SectionService(DataContext context, IMapper mapper, IOptions<AppSettings> appSettings, ILogger<ISectionService> logger)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        public async Task<SectionResponse> GetByIdAsync(int id)
        {
            var section = await GetSection(id);
            return _mapper.Map<SectionResponse>(section);
        }

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

        public async Task<DeleteResponse> DeleteAsync(int id)
        {
            var section = await GetSection(id);
            _context.Sections.Remove(section);
            await _context.SaveChangesAsync();

            return _mapper.Map<DeleteResponse>(section);
        }

        public async Task<IEnumerable<SectionResponse>> GetAllAsync()
        {
            var sections = await _context.Sections.ToListAsync();
            return _mapper.Map<IList<SectionResponse>>(sections);
        }

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

        // Helper method to get a section by its ID
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
