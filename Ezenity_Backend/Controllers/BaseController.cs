using Ezenity_Backend.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ezenity_Backend.Controllers
{
    [Controller]
    public class BaseController : ControllerBase
    {
        // Returns the current authenticated account (null if not logged in)
        public Account Account => (Account)HttpContext.Items["Account"];
        public Skill Skill => (Skill)HttpContext.Items["Skill"];
    }
}
