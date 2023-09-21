using Ezenity_Backend.Attributes;
using Ezenity_Backend.Helpers;
using Ezenity_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ezenity_Backend.Controllers
{
    /// <summary>
    /// Provides a generic base controller to encapsulate CRUD operations.
    /// This controller is designed to be extended by other controllers that require CRUD functionalities.
    /// </summary>
    /// <typeparam name="TEntity">The type representing the data model of the entity.</typeparam>
    /// <typeparam name="TResponse">The type representing the response data shape.</typeparam>
    /// <typeparam name="TCreateRequest">The type representing the creation request model.</typeparam>
    /// <typeparam name="TUpdateRequest">The type representing the update request model.</typeparam>
    /// <typeparam name="TDeleteResponse">The type representing the delete response model.</typeparam>
    public abstract class BaseController<TEntity, TResponse, TCreateRequest, TUpdateRequest, TDeleteResponse> : ControllerBase
    {
        /// <summary>
        /// Gets the entity object associated with the current HTTP context.
        /// </summary>
        protected TEntity Entity => (TEntity) HttpContext.Items["Entity"];

        /// <summary>
        /// Retrieves a single entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to be retrieved.</param>
        /// <returns>An action result containing an API response with the entity data.</returns>
        [HttpGet("{id:int}")]
        public abstract Task<ActionResult<ApiResponse<TResponse>>> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves all entities. Access restricted to Admin role.
        /// </summary>
        /// <returns>An action result containing an API response with a list of entities.</returns>
        [AuthorizeV2("Admin")]
        [HttpGet]
        public abstract Task<ActionResult<ApiResponse<IEnumerable<TResponse>>>> GetAllAsync();

        /// <summary>
        /// Creates a new entity based on the provided creation model. Access restricted to Admin role.
        /// </summary>
        /// <param name="model">The creation model for the new entity.</param>
        /// <returns>An action result containing an API response with the created entity data.</returns>
        [AuthorizeV2("Admin")]
        [HttpPost]
        public abstract Task<ActionResult<ApiResponse<TResponse>>> CreateAsync(TCreateRequest model);

        /// <summary>
        /// Updates an existing entity with the provided data.
        /// </summary>
        /// <param name="id">The identifier of the entity to be updated.</param>
        /// <param name="model">The update model containing the new data.</param>
        /// <returns>An action result containing an API response with the updated entity data.</returns>
        [AuthorizeV2]
        [HttpPut("{id:int}")]
        public abstract Task<ActionResult<ApiResponse<TResponse>>> UpdateAsync(int id, TUpdateRequest model);

        /// <summary>
        /// Deletes an existing entity identified by the given identifier.
        /// </summary>
        /// <param name="DeleteAccountId">The identifier of the entity to be deleted.</param>
        /// <param name="DeletedById">The identifier of the user who is deleting the entity.</param>
        /// <returns>An action result containing an API response with the status of the deletion operation.</returns>
        [AuthorizeV2]
        [HttpDelete("{DeleteAccountId:int}")]
        public abstract Task<ActionResult<ApiResponse<TDeleteResponse>>> DeleteAsync(int DeleteAccountId, int DeletedById);
    }

}
