using Ezenity_Backend.Entities.Sections;
using Ezenity_Backend.Models;
using Ezenity_Backend.Models.Sections;

namespace Ezenity_Backend.Services.Common
{
    public interface ISectionService : IBaseService<Section, SectionResponse, CreateSectionRequest, UpdateSectionRequest, DeleteResponse>
    {
    }
}
