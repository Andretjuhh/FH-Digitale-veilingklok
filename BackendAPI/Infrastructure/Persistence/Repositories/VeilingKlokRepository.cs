using Application.Common.Exceptions;
using Application.Common.Models;
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

    public async Task AddAsync(VeilingKlok veilingKlok)
    {
        await _dbContext.Veilingklokken.AddAsync(veilingKlok);
    }

    public async Task<VeilingKlokStatus?> GetStatusAsync(Guid klokId, CancellationToken ct)
    {
        var klok = await _dbContext.Veilingklokken
            .AsNoTracking()
            .FirstOrDefaultAsync(vk => vk.Id == klokId, ct);

        if (klok == null)
            return null;

        return klok.Status;
    }

    public async Task<VeilingKlok?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Veilingklokken.FirstOrDefaultAsync(vk => vk.Id == id);
    }

    public async Task<IEnumerable<VeilingKlok>> GetAllByStatusAsync(VeilingKlokStatus status, CancellationToken ct)
    {
        return await _dbContext.Veilingklokken.Where(vk => vk.Status == status).ToListAsync(ct);
    }

    public async Task<IEnumerable<VeilingKlok>> GetAllByMeesterIdAsync(Guid meesterId)
    {
        return await _dbContext
            .Veilingklokken.Where(vk => vk.VeilingmeesterId == meesterId)
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
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    #region Advance Queries

    // Queries that return VeilingKlok along with BidsInfo
    private IQueryable<(VeilingKlok VeilingKlok, int BidCount)> QueryProductWithInfo()
    {
        return _dbContext
            .Veilingklokken.Select(vk => new
            {
                VeilingKlok = vk,
                BidsCount = _dbContext.OrderItems.Count(oi => oi.VeilingKlokId == vk.Id)
            })
            .Select(x => ValueTuple.Create(x.VeilingKlok, x.BidsCount));
    }

    public async Task<(VeilingKlok VeilingKlok, int BidCount)?> GetByIdWithBidsCount(Guid id)
    {
        var result = await QueryProductWithInfo().FirstOrDefaultAsync(x => x.VeilingKlok.Id == id);

        if (result.VeilingKlok == null)
            return null;

        return result;
    }

    #endregion
}