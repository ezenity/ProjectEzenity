using AutoMapper;
using Ezenity_Backend.Entities;
using Ezenity_Backend.Helpers;
using Ezenity_Backend.Models.Skills;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ezenity_Backend.Services
{
    public class SkillService : ISkillService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public SkillService(DataContext context, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public SkillResponse GetById(int id)
        {
            var skill = getSkill(id);
            return _mapper.Map<SkillResponse>(skill);
        }

        public SkillResponse Create(CreateSkillRequest model)
        {
            // Validate
            if (_context.Skills.Any(x => x.Title == model.Title))
                throw new AppException($"Skill '{model.Title}' already exsist, please try a different title.");

            // Map model to new skill object
            var skill = _mapper.Map<Skill>(model);
            skill.Created = DateTime.UtcNow;
//            skill.Verified = DateTime.UtcNow; // Might not be needed

            // Save skill
            _context.Skills.Add(skill);
            _context.SaveChanges();

            return _mapper.Map<SkillResponse>(skill);
        }

        public void Delete(int id)
        {
            var skill = getSkill(id);
            _context.Skills.Remove(skill);
            _context.SaveChanges();
        }

        public IEnumerable<SkillResponse> GetAll()
        {
            var skills = _context.Skills;
            return _mapper.Map<IList<SkillResponse>>(skills);
        }

        public SkillResponse Update(int id, UpdateSkillRequest model)
        {
            var skill = getSkill(id);

            // Validate
            if(skill.Title != model.Title && _context.Skills.Any(x => x.Title == model.Title))
                throw new AppException($"Skill '{model.Title}' already exsist, please try a different title.");

            // Copy model to account and save
            _mapper.Map(model, skill);
            skill.Updated = DateTime.UtcNow;
            _context.Skills.Update(skill);
            _context.SaveChanges();

            return _mapper.Map<SkillResponse>(skill);
        }

        /**
         * Private Helper Methods
         */
        private Skill getSkill(int id)
        {
            var skill = _context.Skills.Find(id);
            if (skill == null) throw new KeyNotFoundException("Skill not found");
            return skill;
        }
    }
}
