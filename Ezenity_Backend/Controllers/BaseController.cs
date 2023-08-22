using Ezenity_Backend.Entities;
using Ezenity_Backend.Entities.Accounts;
using Ezenity_Backend.Models.ExpertiseCompentencies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ezenity_Backend.Controllers
{
    public abstract class BaseController<TEntity, TResponse, TCreateRequest, TUpdateRequest> : ControllerBase
    {
        protected TEntity Entity => (TEntity)HttpContext.Items["Entity"];

        // Common CRUB methods
        [HttpGet("{id:int}")]
        public abstract ActionResult<TResponse> GetById(int id);

        [HttpGet]
        public abstract ActionResult<IEnumerable<TResponse>> GetAll();

        [HttpPost]
        public abstract ActionResult<TResponse> Create(TCreateRequest model);

        [HttpPut("{id:int}")]
        public abstract ActionResult<TResponse> Update(int id, TUpdateRequest model);

        [HttpDelete("{id:int}")]
        public abstract IActionResult Delete(int id);
    }

    [ApiController]
    public class BaseController : ControllerBase
    {
        // Returns the current authenticated account (null if not logged in)
        public Account Account => (Account)HttpContext.Items["Account"];
        public Skill Skill => (Skill)HttpContext.Items["Skill"];
        public EC EC => (EC)HttpContext.Items["EC"];
    }
}
