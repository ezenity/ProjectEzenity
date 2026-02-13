using Ezenity.Core.Entities.EmailTemplates;
using Ezenity.Domain.Entities.EmailTemplates; // adjust namespace to your EmailTemplate entity
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ezenity.Application.Abstractions.Persistence;

public interface IEmailTemplateRepository
{
    Task<EmailTemplate?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<EmailTemplate?> GetByNameAsync(string templateName, CancellationToken ct = default);

    Task<bool> ExistsByNameAsync(string templateName, CancellationToken ct = default);

    Task AddAsync(EmailTemplate template, CancellationToken ct = default);
    void Update(EmailTemplate template);
    void Remove(EmailTemplate template);

    Task<IReadOnlyList<EmailTemplate>> ListAsync(CancellationToken ct = default);
}
