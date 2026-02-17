using Ezenity.Contracts;
using Ezenity.Contracts.Models.Sections;
using static System.Collections.Specialized.BitVector32;

namespace Ezenity.Application.Features.Sections;

/// <summary>
/// Service for managing website sections.
/// </summary>
public interface ISectionService : IBaseService<Section, SectionResponse, CreateSectionRequest, UpdateSectionRequest, DeleteResponse>
{
    Task<CreateSectionWithAdditonalRequest> CreateWithAdditionalAsync(CreateSectionWithAdditonalRequest model);
}
