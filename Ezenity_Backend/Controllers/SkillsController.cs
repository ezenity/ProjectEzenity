using AutoMapper;
using Ezenity_Backend.Entities;
using Ezenity_Backend.Helpers;
using Ezenity_Backend.Models.Skills;
using Ezenity_Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ezenity_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SkillsController : BaseController
    {
        private readonly ISkillService _skillService;
        private readonly IMapper _mapper;

        public SkillsController(ISkillService skillProgressService, IMapper mapper)
        {
            _skillService = skillProgressService;
            _mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public ActionResult<SkillResponse> GetById(int id)
        {
            var skill = _skillService.GetById(id);
            return Ok(skill);
        }

        [HttpGet]
        public ActionResult<IEnumerable<SkillResponse>> GetAll()
        {
            var skills = _skillService.GetAll();
            return Ok(skills);
        }

        [HttpPost]
        [Authorize(Role.Admin)]
        public ActionResult<SkillResponse> Create(CreateSkillRequest model)
        {
            var skill = _skillService.Create(model);
            return Ok(skill);
        }

        [Authorize]
        /*[HttpPost("{id:int}")]*/
        [HttpPut("{id:int}")]
        public ActionResult<SkillResponse> Update(int id, UpdateSkillRequest model)
        {
            var skill = _skillService.Update(id, model);
            return Ok(skill);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            _skillService.Delete(id);
            return Ok(new { message = "Skill deleted seuccessfully." });
        }


    }
}
