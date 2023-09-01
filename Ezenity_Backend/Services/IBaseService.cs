using System.Collections.Generic;

namespace Ezenity_Backend.Services
{
    public interface IBaseService<TEntity, TResponse, TCreateRequest, TUpdateRequest>
        where TEntity : class
    {
        TResponse GetById(int id);
        TResponse Create(TCreateRequest model);
        void Delete(int id);
        IEnumerable<TResponse> GetAll();
        TResponse Update(int id, TUpdateRequest model);
    }
}
