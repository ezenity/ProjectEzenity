using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ezenity.Domain.Entities.Sections; // adjust namespace to your Section entity

namespace Ezenity.Application.Abstractions.Persistence;

public interface ISectionRepository
{
    Task<Section?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Section?> GetByTitleAsync(string title, CancellationToken ct = default);

    Task<bool> ExistsByTitleAsync(string title, CancellationToken ct = default);

    Task AddAsync(Section section, CancellationToken ct = default);
    void Update(Section section);
    void Remove(Section section);

    Task<IReadOnlyList<Section>> ListAsync(CancellationToken ct = default);
}
