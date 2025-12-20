using Application.Common.Exceptions;
using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class KwekerRepository : UserRepository, IKwekerRepository
{
    public KwekerRepository(AppDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task AddAsync(Kweker kweker)
    {
        await _dbContext.Kwekers.AddAsync(kweker);
    }

    public void Update(Kweker kweker)
    {
        _dbContext.Kwekers.Update(kweker);
    }

    public async Task<Kweker?> GetKwekerByIdAsync(Guid accountId)
    {
        return await _dbContext
            .Kwekers.Include(k => k.Adress)
            .FirstOrDefaultAsync(k => k.Id == accountId);
    }
}