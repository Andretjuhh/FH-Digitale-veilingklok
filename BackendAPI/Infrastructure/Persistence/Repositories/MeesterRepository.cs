using Application.Common.Exceptions;
using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MeesterRepository : UserRepository, IMeesterRepository
{
    public MeesterRepository(AppDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task AddAsync(Veilingmeester meester)
    {
        await _dbContext.Veilingmeesters.AddAsync(meester);
    }

    public void Update(Veilingmeester meester)
    {
        _dbContext.Veilingmeesters.Update(meester);
    }

    public async Task DeleteMeesterAsync(Guid id)
    {
        var meester = await GetMeesterByIdAsync(id);
        if (meester != null)
            _dbContext.Veilingmeesters.Remove(meester);
    }

    public async Task<Veilingmeester?> GetMeesterByIdAsync(Guid accountId)
    {
        return await _dbContext.Veilingmeesters.FirstOrDefaultAsync(m => m.Id == accountId);
    }
}