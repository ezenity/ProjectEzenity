using Ezenity_Backend.Models.Pages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ezenity_Backend.Services
{
    /// <summary>
    /// Interface for common CRUD operations.
    /// </summary>
    public interface IBaseService<TEntity, TResponse, TCreateRequest, TUpdateRequest, TDeleteResponse>
        where TEntity : class
    {
        /// <summary>
        /// Gets all entities.
        /// </summary>
        Task<IEnumerable<TResponse>> GetAllAsync();
        Task<PagedResult<TResponse>> GetAllAsync(string? name, string? searchQuery, int pageNumber, int pageSize);

        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        Task<TResponse> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new entity based on the provided model.
        /// </summary>
        Task<TResponse> CreateAsync(TCreateRequest model);

        /// <summary>
        /// Updates an entity based on the provided ID and update model.
        /// </summary>
        Task<TResponse> UpdateAsync(int id, TUpdateRequest model);

        /// <summary>
        /// Deletes an entity by its ID.
        /// </summary>
        Task<TDeleteResponse> DeleteAsync(int id);
    }
}
