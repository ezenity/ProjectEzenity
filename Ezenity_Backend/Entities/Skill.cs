using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ezenity_Backend.Entities
{
    public class Skill
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Percentage { get; set; }
        /*public DateTime? Verified { get; set; }*/
        /*public bool IsVerified => Verified.HasValue;*/
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
