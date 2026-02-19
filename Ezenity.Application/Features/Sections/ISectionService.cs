using Ezenity.Contracts;
using Ezenity.Contracts.Models.Sections;
using Ezenity.Domain.Entities.Sections;

namespace Ezenity.Application.Features.Sections;

/// <summary>
/// Service for managing website sections.
/// </summary>
public interface ISectionService : IBaseService<Section, SectionResponse, CreateSectionRequest, UpdateSectionRequest, DeleteResponse>
{
    Task<SectionResponse> CreateWithAdditionalAsync(CreateSectionWithAdditonalRequest model, CancellationToken ct = default);
}
