using Ezenity_Backend.Entities.Sections;
using Ezenity_Backend.Models;
using Ezenity_Backend.Models.Sections;
using System.Threading.Tasks;

namespace Ezenity_Backend.Services.Common
{
    /// <summary>
    /// Service for managing website sections.
    /// </summary>
    public interface ISectionService : IBaseService<Section, SectionResponse, CreateSectionRequest, UpdateSectionRequest, DeleteResponse>
    {
        Task<CreateSectionWithAdditonalRequest> CreateWithAdditionalAsync(CreateSectionWithAdditonalRequest model);
    }
}
