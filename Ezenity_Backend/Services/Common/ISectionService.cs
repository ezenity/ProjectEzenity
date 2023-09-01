using Ezenity_Backend.Entities;
using Ezenity_Backend.Models.Common.Accounts;
using Ezenity_Backend.Models.Common.Sections;

namespace Ezenity_Backend.Services.Common
{
    public interface ISectionService : IBaseService<ISection, ISectionResponse, ICreateSectionRequest, IUpdateAccountRequest>
    {
    }
}
