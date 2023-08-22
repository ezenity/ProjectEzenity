using Ezenity_Backend.Entities.Sections;
using Ezenity_Backend.Models.Sections;

namespace Ezenity_Backend.Services.Sections
{
    public interface ISectionService : IBaseService<Section, SectionResponse, CreateSectionRequest, UpdateSectionRequest>
    {
    }
}
