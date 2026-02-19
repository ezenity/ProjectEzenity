using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ezenity.Application.Abstractions.Configuration;
using Ezenity.Application.Abstractions.Persistence;
using Ezenity.Application.Common.Exceptions;
using Ezenity.Contracts;
using Ezenity.Contracts.Models.Pages;
using Ezenity.Contracts.Models.Sections;
using Ezenity.Domain.Entities.Sections;
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Logging;

namespace Ezenity.Application.Features.Sections;

/// <summary>
/// Service to handle CRUD operations for sections.
/// </summary>
public class SectionService : ISectionService
{
    /// <summary>
    /// Provides data access to the application's data store.
    /// </summary>
    private readonly IUnitOfWork _uow;
    private readonly ISectionRepository _sections;

    /// <summary>
    /// Provides object-object mapping functionality.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    /// Provides access to application settings.
    /// </summary>
    private readonly IAppSettings _appSettings;

    /// <summary>
    /// Provides logging capabilities.
    /// </summary>
    private readonly ILogger<SectionService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SectionService"/> class.
    /// </summary>
    /// <param name="context">Data context for database interaction.</param>
    /// <param name="mapper">Object mapper for model transformation.</param>
    /// <param name="appSettings">Application settings.</param>
    /// <param name="logger">Logger instance.</param>
    public SectionService(IUnitOfWork uow, ISectionRepository sections, IMapper mapper, IAppSettings appSettings, ILogger<SectionService> logger)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _sections = sections ?? throw new ArgumentNullException(nameof(sections));
        _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
        _appSettings = appSettings ?? throw new ArgumentException(nameof(appSettings));
        _logger = logger ?? throw new ArgumentException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a section by its identifier.
    /// </summary>
    /// <param name="id">Section identifier.</param>
    /// <returns>The section details.</returns>
    //var section = await GetSection(id);
    //return _mapper.Map<SectionResponse>(section);
    public async Task<SectionResponse> GetByIdAsync(int id)
    {
        var section = await GetSectionRequiredAsync(id, CancellationToken.None);
        return _mapper.Map<SectionResponse>(await GetSection(id));
    }

    /// <summary>
    /// Creates a new section.
    /// </summary>
    /// <param name="model">Data for creating the section.</param>
    /// <returns>The newly created section.</returns>
    public async Task<SectionResponse> CreateAsync(CreateSectionRequest model)
    {
        var ct = CancellationToken.None;

        if (await _sections.ExistsByTitleAsync(model.Title, ct))
            throw new AppException($"The Section Title '{model.Title}' already exists. Please try a different title.");

        await using var tx = await _uow.BeginTransactionAsync(ct);

        try
        {
            var section = _mapper.Map<Section>(model);
            section.Title = model.Title.Trim();
            section.Created = DateTime.UtcNow;
            section.IsActive = true;

            await _sections.AddAsync(section, ct);
            await _uow.SaveChangesAsync(ct);

            await tx.CommitAsync(ct);

            return _mapper.Map<SectionResponse>(section);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            _logger.LogError(ex, "Error creating section");
            throw;
        }
    }

    /// <summary>
    /// Asynchronously creates a new section with additional information.
    /// </summary>
    /// <param name="model">The request model containing the details for the new section.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created section details.</returns>
    /// <exception cref="AppException">Thrown when the section title already exists.</exception>
    public async Task<SectionResponse> CreateWithAdditionalAsync(CreateSectionWithAdditonalRequest model, CancellationToken ct = default)
    {
        if (await _sections.ExistsByTitleAsync(model.Title, ct))
            throw new AppException($"The Section Title '{model.Title}' already exists. Please try a different title.");

        await using var tx = await _uow.BeginTransactionAsync(ct);

        try
        {
            var section = _mapper.Map<Section>(model);
            section.Title = model.Title.Trim();
            section.Created = DateTime.UtcNow;

            await _sections.AddAsync(section, ct);
            await _uow.SaveChangesAsync(ct);

            await tx.CommitAsync(ct);

            return _mapper.Map<SectionResponse>(section);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            _logger.LogError(ex, "Error creating section with additional data");
            throw;
        }
    }

    /// <summary>
    /// Deletes a section by its identifier.
    /// </summary>
    /// <param name="id">Section identifier.</param>
    /// <returns>Response after deletion.</returns>
    public async Task<DeleteResponse> DeleteAsync(int id)
    {
        var ct = CancellationToken.None;

        await using var tx = await _uow.BeginTransactionAsync(ct);

        try
        {
            var section = await GetSectionRequiredAsync(id, ct);

            _sections.Remove(section);
            await _uow.SaveChangesAsync(ct);

            await tx.CommitAsync(ct);

            // If your DeleteResponse expects different fields, map accordingly
            return _mapper.Map<DeleteResponse>(section);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            _logger.LogError(ex, "Error deleting section id={SectionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all available sections.
    /// </summary>
    /// <returns>List of all sections.</returns>
    public async Task<IEnumerable<SectionResponse>> GetAllAsync()
    {
        var items = await _sections.ListAsync(CancellationToken.None);
        return _mapper.Map<IReadOnlyList<SectionResponse>>(items);
    }


    public async Task<PagedResult<SectionResponse>> GetAllAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new AppException("PagedResult pageNumber and pageSize must be greater than zero.");

        var query = _sections.Query().AsNoTracking();

        // We used "name" in other services; for sections we’ll interpret it as Title exact match
        if (!string.IsNullOrWhiteSpace(name))
        {
            var title = name.Trim();
            query = query.Where(s => s.Title == title);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var q = searchQuery.Trim();

            // EF Core: translates to LIKE on most providers
            query = query.Where(s =>
                s.Title.Contains(q) ||
                s.ContentType.Contains(q) ||
                s.Layout.Contains(q));
        }

        var total = await query.CountAsync();
        if (total == 0)
            throw new ResourceNotFoundException("No sections were found.");

        var data = await query
            .OrderByDescending(s => s.Created)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ProjectTo<SectionResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedResult<SectionResponse>
        {
            Data = data,
            Pagination = new PaginationMetadata(total, pageSize, pageNumber)
        };
    }

    /// <summary>
    /// Updates an existing section.
    /// </summary>
    /// <param name="id">Section identifier.</param>
    /// <param name="model">Data for updating the section.</param>
    /// <returns>The updated section.</returns>
    public async Task<SectionResponse> UpdateAsync(int id, UpdateSectionRequest model)
    {
        var ct = CancellationToken.None;

        await using var tx = await _uow.BeginTransactionAsync(ct);

        try
        {
            var section = await GetSectionRequiredAsync(id, ct);

            // If Title changed, enforce uniqueness
            if (!string.Equals(section.Title, model.Title, StringComparison.Ordinal) &&
                await _sections.ExistsByTitleAsync(model.Title, ct))
            {
                throw new AppException($"The Section Title '{model.Title}' already exists. Please try a different title.");
            }

            _mapper.Map(model, section);
            section.Title = model.Title.Trim();
            section.Updated = DateTime.UtcNow;

            _sections.Update(section);
            await _uow.SaveChangesAsync(ct);

            await tx.CommitAsync(ct);

            return _mapper.Map<SectionResponse>(section);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            _logger.LogError(ex, "Error updating section id={SectionId}", id);
            throw;
        }
    }

    private async Task<Section> GetSectionRequiredAsync(int id, CancellationToken ct)
    {
        var section = await _sections.GetByIdAsync(id, ct);
        if (section is null)
            throw new ResourceNotFoundException($"Section with ID {id} was not found.");
        return section;
    }
}
