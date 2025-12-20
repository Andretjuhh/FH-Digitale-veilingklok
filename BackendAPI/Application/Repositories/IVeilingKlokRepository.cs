using Application.Common.Models;
using Domain.Entities;
using Domain.Enums;

namespace Application.Repositories;

public interface IVeilingKlokRepository
{
    Task<VeilingKlokStatus?> GetStatusAsync(Guid klokId, CancellationToken ct);
    Task<VeilingKlok?> GetByIdAsync(Guid id);
    Task<(VeilingKlok VeilingKlok, int BidCount)?> GetByIdWithBidsCount(Guid id);

    Task AddAsync(VeilingKlok veilingKlok);
    void Update(VeilingKlok veilingKlok);

    Task<IEnumerable<VeilingKlok>> GetAllByMeesterIdAsync(Guid meesterId);
    Task<IEnumerable<VeilingKlok>> GetAllByStatusAsync(VeilingKlokStatus status, CancellationToken ct);

    Task<(IEnumerable<VeilingKlok> Items, int TotalCount)> GetAllWithFilterAsync(
        VeilingKlokStatus? statusFilter,
        DateTime? scheduledAfter,
        Guid? meesterId,
        string? country,
        int pageNumber,
        int pageSize
    );
}