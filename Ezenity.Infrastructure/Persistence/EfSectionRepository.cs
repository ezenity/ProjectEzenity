using Ezenity.Application.Abstractions.Persistence;
using Ezenity.Domain.Entities.Sections;
using Ezenity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ezenity.Infrastructure.Persistence;

public sealed class EfSectionRepository : ISectionRepository
{
    private readonly DataContext _db;

    public EfSectionRepository(DataContext db) => _db = db;

    public IQueryable<Section> Query()
        => _db.Sections.AsQueryable();

    public Task<Section?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Sections.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Section?> GetByTitleAsync(string title, CancellationToken ct = default)
    {
        var t = NormalizeTitle(title);
        return _db.Sections.FirstOrDefaultAsync(x => x.Title == t, ct);
    }

    public Task<bool> ExistsByTitleAsync(string title, CancellationToken ct = default)
    {
        var t = NormalizeTitle(title);
        return _db.Sections.AnyAsync(x => x.Title == t, ct);
    }

    public Task AddAsync(Section section, CancellationToken ct = default) =>
        _db.Sections.AddAsync(section, ct).AsTask();

    public void Update(Section section) =>
        _db.Sections.Update(section);

    public void Remove(Section section) =>
        _db.Sections.Remove(section);

    public async Task<IReadOnlyList<Section>> ListAsync(CancellationToken ct = default) =>
        await _db.Sections
            .AsNoTracking()
            .OrderByDescending(x => x.Created) // if we have CreatedUtc use that
            .ToListAsync(ct);

    private static string NormalizeTitle(string title) =>
        (title ?? string.Empty).Trim();
}
