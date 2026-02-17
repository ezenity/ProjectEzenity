using Ezenity.Application.Abstractions.Persistence;
using Ezenity.Domain.Entities.EmailTemplates;
using Ezenity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ezenity.Infrastructure.Persistence;

public sealed class EfEmailTemplateRepository : IEmailTemplateRepository
{
    private readonly DataContext _db;

    public EfEmailTemplateRepository(DataContext db)
    {
        _db = db;
    }

    public Task<EmailTemplate?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.EmailTemplates.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<EmailTemplate?> GetByNameAsync(string templateName, CancellationToken ct = default) =>
        _db.EmailTemplates.FirstOrDefaultAsync(x => x.TemplateName == templateName, ct);

    public Task<bool> ExistsByNameAsync(string templateName, CancellationToken ct = default) =>
        _db.EmailTemplates.AnyAsync(x => x.TemplateName == templateName, ct);

    public Task AddAsync(EmailTemplate template, CancellationToken ct = default) =>
        _db.EmailTemplates.AddAsync(template, ct).AsTask();

    public void Update(EmailTemplate template) =>
        _db.EmailTemplates.Update(template);

    public void Remove(EmailTemplate template) =>
        _db.EmailTemplates.Remove(template);

    public async Task<IReadOnlyList<EmailTemplate>> ListAsync(CancellationToken ct = default) =>
        await _db.EmailTemplates
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt) // if yours is CreatedUtc rename here
            .ToListAsync(ct);
}
