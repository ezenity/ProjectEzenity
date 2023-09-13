using Ezenity_Backend.Helpers;
using Ezenity_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ezenity_Backend.Controllers
{
    public abstract class BaseController<TEntity, TResponse, TCreateRequest, TUpdateRequest, TDeleteResponse> : ControllerBase
    {
        protected TEntity Entity => (TEntity) HttpContext.Items["Entity"];

        // Common CRUD methods
        [HttpGet("{id:int}")]
        public abstract Task<ActionResult<ApiResponse<TResponse>>> GetByIdAsync(int id);

        [Authorize("Admin")]
        [HttpGet]
        public abstract Task<ActionResult<ApiResponse<IEnumerable<TResponse>>>> GetAllAsync();

        [Authorize("Admin")]
        [HttpPost]
        public abstract Task<ActionResult<ApiResponse<TResponse>>> CreateAsync(TCreateRequest model);

        [Authorize]
        [HttpPut("{id:int}")]
        public abstract Task<ActionResult<ApiResponse<TResponse>>> UpdateAsync(int id, TUpdateRequest model);

        [Authorize]
        [HttpDelete("{id:int}")]
        public abstract Task<ActionResult<ApiResponse<TDeleteResponse>>> DeleteAsync(int DeleteAccountId, int DeletedById);
    }

}
