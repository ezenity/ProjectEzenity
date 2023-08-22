using Ezenity_Backend.Entities.Accounts;
using Ezenity_Backend.Entities.Sections;
using Ezenity_Backend.Helpers;
using Ezenity_Backend.Models.Sections;
using Ezenity_Backend.Services.Sections;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Ezenity_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SectionsController : BaseController<Section, SectionResponse, CreateSectionRequest, UpdateSectionRequest>
    {
        private readonly ISectionService _sectionService;

        public SectionsController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

        public override ActionResult<SectionResponse> GetById(int id)
        {
            var section = _sectionService.GetById(id);

            if (section == null)
            {
                return NotFound();
            }

            return Ok(section);
        }

        public override ActionResult<IEnumerable<SectionResponse>> GetAll()
        {
            var sections = _sectionService.GetAll();
            return Ok(sections);
        }

        [Authorize(Role.Admin)]
        public override ActionResult<SectionResponse> Create(CreateSectionRequest model)
        {
            var section = _sectionService.Create(model);
            return Ok(section);
        }

        [Authorize(Role.Admin)]
        public override ActionResult<SectionResponse> Update(int id, UpdateSectionRequest model)
        {
            var existingSection = _sectionService.GetById(id);
            if (existingSection == null)
            {
                return NotFound();
            }

            var updatedSection = _sectionService.Update(id, model);
            return Ok(updatedSection);
        }

        [Authorize(Role.Admin)]
        public override IActionResult Delete(int id)
        {
            _sectionService.Delete(id);
            return Ok(new { message = "Section deleted successfully." } );
        }
    }
}
