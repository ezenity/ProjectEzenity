using AutoMapper;
using Ezenity_Backend.Helpers;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

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

        public abstract TResponse GetById(int id);
        public abstract TResponse Create(TCreateRequest model);
        public abstract void Delete(int id);
        public abstract IEnumerable<TResponse> GetAll();
        public abstract TResponse Update(int id, TUpdateRequest model);
    }
}
