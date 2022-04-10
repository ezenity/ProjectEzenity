using Ezenity_Backend.Models.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ezenity_Backend.Services
{
    public interface ISkillService
    {
        IEnumerable<SkillResponse> GetAll();
        SkillResponse GetById(int id);
        SkillResponse Create(CreateSkillRequest model);
        SkillResponse Update(int id, UpdateSkillRequest model);
        void Delete(int id);
    }
}
