using AutoMapper;
using Ezenity_Backend.Helpers;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ezenity_Backend.Services
{
    public abstract class BaseService<TEntity, TResponse, TCreateRequest, TUpdateRequest> : IBaseService<TEntity, TResponse, TCreateRequest, TUpdateRequest>
        where TEntity : class
    {
        protected readonly DataContext _context;
        protected readonly IMapper _mapper;
        protected readonly AppSettings _appSettings;
        

        public BaseService(DataContext context, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public abstract Task<IEnumerable<TResponse>> GetAllAsync();
        public abstract Task<TResponse> GetByIdAsync(int id);
        public abstract Task<TResponse> CreateAsync(TCreateRequest model);
        public abstract Task<TResponse> UpdateAsync(int id, TUpdateRequest model);
        public abstract Task<TResponse> DeleteAsync(int id);
    }
}
