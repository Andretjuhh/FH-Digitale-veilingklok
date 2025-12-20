using Application.Common.Exceptions;
using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class KoperRepository : UserRepository, IKoperRepository
{
    public KoperRepository(AppDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task AddAsync(Koper koper)
    {
        await _dbContext.Kopers.AddAsync(koper);
    }

    public void Update(Koper koper)
    {
        _dbContext.Kopers.Update(koper);
    }

    public async Task<Koper?> GetKoperByIdAsync(Guid id)
    {
        return await _dbContext.Kopers.Include(k => k.Adresses).FirstOrDefaultAsync(k => k.Id == id);
    }
}