using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VeilingKlokRepository : IVeilingKlokRepository
{
    private readonly AppDbContext _dbContext;

    public VeilingKlokRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Update(VeilingKlok veilingKlok)
    {
        _dbContext.Veilingklokken.Update(veilingKlok);
    }

    public void Delete(VeilingKlok veilingKlok)
    {
        _dbContext.Veilingklokken.Remove(veilingKlok);
    }

    public async Task AddAsync(VeilingKlok veilingKlok)
    {
        await _dbContext.Veilingklokken.AddAsync(veilingKlok);
    }

    public async Task<VeilingKlokStatus?> GetStatusAsync(Guid klokId, CancellationToken ct)
    {
        var klok = await _dbContext
            .Veilingklokken.AsNoTracking()
            .FirstOrDefaultAsync(vk => vk.Id == klokId, ct);

        if (klok == null)
            return null;

        return klok.Status;
    }

    public async Task<VeilingKlok?> GetByIdAsync(Guid id)
    {
        return await _dbContext
            .Veilingklokken.Include(vk => vk.VeilingKlokProducts)
            .FirstOrDefaultAsync(vk => vk.Id == id);
    }

    public async Task<IEnumerable<VeilingKlok>> GetAllByStatusAsync(
        VeilingKlokStatus status,
        CancellationToken ct
    )
    {
        return await _dbContext
            .Veilingklokken.Include(vk => vk.VeilingKlokProducts)
            .Where(vk => vk.Status == status)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<VeilingKlok>> GetAllByMeesterIdAsync(Guid meesterId)
    {
        return await _dbContext
            .Veilingklokken.Include(vk => vk.VeilingKlokProducts)
            .Where(vk => vk.VeilingmeesterId == meesterId)
            .ToListAsync();
    }

    public async Task<(IEnumerable<VeilingKlok> Items, int TotalCount)> GetAllWithFilterAsync(
        VeilingKlokStatus? statusFilter,
        DateTime? scheduledAfter,
        Guid? meesterId,
        string? country,
        int pageNumber,
        int pageSize
    )
    {
        var query = _dbContext.Veilingklokken.AsQueryable();

        if (statusFilter.HasValue)
            query = query.Where(vk => vk.Status == statusFilter.Value);

        if (meesterId.HasValue)
            query = query.Where(vk => vk.VeilingmeesterId == meesterId.Value);

        if (scheduledAfter.HasValue)
            query = query.Where(vk => vk.ScheduledAt >= scheduledAfter.Value);

        if (!string.IsNullOrEmpty(country))
            query =
                from vk in query
                join vm in _dbContext.Veilingmeesters on vk.VeilingmeesterId equals vm.Id
                where vm.CountryCode == country
                select vk;

        var totalCount = await query.CountAsync();
        var items = await query
            .Include(vk => vk.VeilingKlokProducts)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, totalCount);
    }

    public async Task<(
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
    )
    {
        // Start with base query on VeilingKlok
        var query = _dbContext.Veilingklokken.AsQueryable();

        // Apply filters
        if (statusFilter.HasValue)
            query = query.Where(vk => vk.Status == statusFilter.Value);

        if (meesterId.HasValue)
            query = query.Where(vk => vk.VeilingmeesterId == meesterId.Value);

        if (!string.IsNullOrEmpty(region))
            query = query.Where(vk => vk.RegionOrState.Contains(region));

        if (scheduledAfter.HasValue)
            query = query.Where(vk => vk.ScheduledAt >= scheduledAfter.Value);

        if (scheduledBefore.HasValue)
            query = query.Where(vk => vk.ScheduledAt <= scheduledBefore.Value);

        if (startedAfter.HasValue)
            query = query.Where(vk => vk.StartedAt >= startedAfter.Value);

        if (startedBefore.HasValue)
            query = query.Where(vk => vk.StartedAt <= startedBefore.Value);

        if (endedAfter.HasValue)
            query = query.Where(vk => vk.EndedAt >= endedAfter.Value);

        if (endedBefore.HasValue)
            query = query.Where(vk => vk.EndedAt <= endedBefore.Value);

        // Get total count
        var totalCount = await query.CountAsync();

        // Get paginated items with VeilingKlokProducts and bid counts
        var items = await query
            .Include(vk => vk.VeilingKlokProducts)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(vk => new
            {
                VeilingKlok = vk,
                BidsCount = _dbContext.OrderItems.Count(oi => oi.VeilingKlokId == vk.Id),
            })
            .ToListAsync();

        // Convert to tuple
        var result = items.Select(x => (x.VeilingKlok, x.BidsCount)).ToList();

        return (result, totalCount);
    }

    public async Task<
        IEnumerable<(VeilingKlok VeilingKlok, int BidCount)>
    > GetAllByMeesterIdWithBidsCountAsync(Guid meesterId)
    {
        return await _dbContext
            .Veilingklokken.Include(vk => vk.VeilingKlokProducts)
            .Where(vk => vk.VeilingmeesterId == meesterId)
            .Select(vk => new
            {
                VeilingKlok = vk,
                BidsCount = _dbContext.OrderItems.Count(oi => oi.VeilingKlokId == vk.Id),
            })
            .Select(x => ValueTuple.Create(x.VeilingKlok, x.BidsCount))
            .ToListAsync();
    }

    public async Task<(VeilingKlok VeilingKlok, int BidCount)?> GetByIdWithBidsCount(Guid id)
    {
        var veilingKlok = await _dbContext
            .Veilingklokken.Include(vk => vk.VeilingKlokProducts)
            .FirstOrDefaultAsync(vk => vk.Id == id);

        if (veilingKlok == null)
            return null;

        var bidCount = await _dbContext.OrderItems.CountAsync(oi => oi.VeilingKlokId == id);

        return (veilingKlok, bidCount);
    }
}
