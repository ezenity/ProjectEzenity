using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ezenity_Backend.Models.Skills
{
    public class SkillResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Percentage { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        /*public bool IsVerified { get; set; }*/
    }
}
