using AutoMapper;
using Ezenity_Backend.Entities.Sections;
using Ezenity_Backend.Helpers;
using Ezenity_Backend.Models.Sections;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ezenity_Backend.Services.Sections
{
    public class SectionService : BaseService<Section, SectionResponse, CreateSectionRequest, UpdateSectionRequest>, ISectionService
    {
        public SectionService(DataContext context, IMapper mapper, IOptions<AppSettings> appSettings)
            : base(context, mapper, appSettings)
        {
        }

        public override SectionResponse GetById(int id)
        {
            var section = GetSection(id);
            return _mapper.Map<SectionResponse>(section);
        }

        public override SectionResponse Create(CreateSectionRequest model)
        {
            // Validate
            if (_context.Sections.Any(x => x.Title == model.Title))
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
            _context.SaveChanges();

            return _mapper.Map<SectionResponse>(section);
        }

        public override void Delete(int id)
        {
            var section = GetSection(id);
            _context.Sections.Remove(section);
            _context.SaveChanges();
        }

        public override IEnumerable<SectionResponse> GetAll()
        {
            var sections = _context.Sections.ToList();
            return _mapper.Map<IList<SectionResponse>>(sections);
        }

        public override SectionResponse Update(int id, UpdateSectionRequest model)
        {
            var section = GetSection(id);

            // Validate
            if (section.Title != model.Title && _context.Sections.Any(x => x.Title == model.Title))
                throw new AppException($"The Section Title, '{model.Title}', already exist, please try a different title.");

            // Update the common properties
            _mapper.Map(model, section);

            section.Updated = DateTime.UtcNow;

            _context.Sections.Update(section);
            _context.SaveChanges();

            return _mapper.Map<SectionResponse>(section);
        }

        // Helper method to get a section by its ID
        private Section GetSection(int id)
        {
            /*var section = _context.Sections.Include(s => s.Content).SingleOrDefault(s => s.Id == id);*/
            var section = _context.Sections.Find(id);
            if (section == null)
                throw new KeyNotFoundException("Section not found");
            return section;
        }
    }
}
