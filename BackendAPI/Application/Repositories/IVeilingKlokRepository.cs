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
    void Delete(VeilingKlok veilingKlok);

    Task<IEnumerable<VeilingKlok>> GetAllByMeesterIdAsync(Guid meesterId);
    Task<IEnumerable<VeilingKlok>> GetAllByStatusAsync(
        VeilingKlokStatus status,
        CancellationToken ct
    );
    Task<bool> HasActiveVeilingInRegionAsync(
        string region,
        string country,
        Guid excludeVeilingId,
        CancellationToken ct = default
    );

    Task<(IEnumerable<VeilingKlok> Items, int TotalCount)> GetAllWithFilterAsync(
        VeilingKlokStatus? statusFilter,
        DateTime? scheduledAfter,
        Guid? meesterId,
        string? country,
        int pageNumber,
        int pageSize
    );

    Task<(
        IEnumerable<(VeilingKlok VeilingKlok, int BidCount)> Items,
        int TotalCount
    )> GetAllWithFilterAndBidsAsync(
        VeilingKlokStatus? statusFilter,
        string? region,
        DateTime? scheduledAfter,
        DateTime? scheduledBefore,
        DateTime? startedAfter,
        DateTime? startedBefore,
        DateTime? endedAfter,
        DateTime? endedBefore,
        Guid? meesterId,
        int pageNumber,
        int pageSize
    );

    Task<IEnumerable<(VeilingKlok VeilingKlok, int BidCount)>> GetAllByMeesterIdWithBidsCountAsync(
        Guid meesterId
    );
}
