using Ezenity.Core.Entities.Sections;
using Ezenity.DTOs.Models;
using Ezenity.DTOs.Models.Sections;

namespace Ezenity.Application.Features.Sections;

/// <summary>
/// Service for managing website sections.
/// </summary>
public interface ISectionService : IBaseService<Section, SectionResponse, CreateSectionRequest, UpdateSectionRequest, DeleteResponse>
{
    Task<CreateSectionWithAdditonalRequest> CreateWithAdditionalAsync(CreateSectionWithAdditonalRequest model);
}
