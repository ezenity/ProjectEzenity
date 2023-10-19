using AutoMapper;
using Ezenity.Core.Interfaces;
using Ezenity.Core.Services;
using Ezenity.DTOs.Models.Pages;
using Ezenity.Infrastructure.Helpers;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ezenity.Infrastructure.Services
{
    /// <summary>
    /// Abstract base service for common CRUD operations.
    /// </summary>
    public abstract class BaseService<TEntity, TResponse, TCreateRequest, TUpdateRequest, TDeleteResponse> : IBaseService<TEntity, TResponse, TCreateRequest, TUpdateRequest, TDeleteResponse>
        where TEntity : class
    {
        /// <summary>
        /// The data context used for database operations.
        /// </summary>
        protected readonly DataContext _context;

        /// <summary>
        /// The AutoMapper instance used for object-object mapping.
        /// </summary>
        protected readonly IMapper _mapper;

        /// <summary>
        /// The application settings.
        /// </summary>
        protected readonly IAppSettings _appSettings;

        /// <summary>
        /// Initializes a new instance of the BaseService class.
        /// </summary>
        /// <param name="context">The data context.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="appSettings">The application settings.</param>
        public BaseService(DataContext context, IMapper mapper, IAppSettings appSettings)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
            _appSettings = appSettings ?? throw new ArgumentException(nameof(appSettings));
        }

        /// <summary>
        /// Gets all entities.
        /// </summary>
        public abstract Task<IEnumerable<TResponse>> GetAllAsync();

        public abstract Task<PagedResult<TResponse>> GetAllAsync(string? name, string? searchQuery, int pageNumber, int pageSize);

        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        public abstract Task<TResponse> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new entity based on the provided model.
        /// </summary>
        public abstract Task<TResponse> CreateAsync(TCreateRequest model);

        /// <summary>
        /// Updates an entity based on the provided ID and update model.
        /// </summary>
        public abstract Task<TResponse> UpdateAsync(int id, TUpdateRequest model);

        /// <summary>
        /// Deletes an entity by its ID.
        /// </summary>
        public abstract Task<TDeleteResponse> DeleteAsync(int id);
    }
}
