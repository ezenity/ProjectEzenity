using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ezenity_Backend.Models.Skills
{
    public class UpdateSkillRequest
    {

        [MinLength(4)]
        public string Title { get; set; }
        public int Percentage { get; set; }

    }
}
