using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ezenity_Backend.Services
{
    public interface IBaseService<TEntity, TResponse, TCreateRequest, TUpdateRequest, TDeleteResponse>
        where TEntity : class
    {
        Task<IEnumerable<TResponse>> GetAllAsync();
        Task<TResponse> GetByIdAsync(int id);
        Task<TResponse> CreateAsync(TCreateRequest model);
        Task<TResponse> UpdateAsync(int id, TUpdateRequest model);
        Task<TDeleteResponse> DeleteAsync(int id);
    }
}
