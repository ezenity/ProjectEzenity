using Ezenity_Backend.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ezenity_Backend.Controllers
{
    public abstract class BaseController<TEntity, TResponse, TCreateRequest, TUpdateRequest> : ControllerBase
    {
        protected TEntity Entity => (TEntity)HttpContext.Items["Entity"];

        // Common CRUD methods
        [HttpGet("{id:int}")]
        public abstract Task<ActionResult<TResponse>> GetByIdAsync(int id);

        [HttpGet]
        public abstract Task<ActionResult<IEnumerable<TResponse>>> GetAllAsync();

        [HttpPost]
        public abstract Task<ActionResult<TResponse>> CreateAsync(TCreateRequest model);

        [HttpPut("{id:int}")]
        public abstract Task<ActionResult<TResponse>> UpdateAsync(int id, TUpdateRequest model);

        [HttpDelete("{id:int}")]
        public abstract Task<IActionResult> DeleteAsync(int id);
    }

}
