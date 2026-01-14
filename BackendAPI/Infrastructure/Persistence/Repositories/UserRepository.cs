using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    protected readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistingAccountAsync(string email)
    {
        return await _dbContext.Users.AnyAsync(a => a.Email == email);
    }

    public async Task<bool> ExistingAccountAsync(Guid accountId)
    {
        return await _dbContext.Users.AnyAsync(a => a.Id == accountId);
    }

    public async Task<Account?> GetByEmailAsync(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<Account?> GetByIdAsync(Guid accountId)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(a => a.Id == accountId);
    }

    public async Task DeleteAccountAsync(Guid id, bool softDelete = true)
    {
        var account = await _dbContext.Users.FirstOrDefaultAsync(a => a.Id == id);
        if (account == null)
            return;

        if (softDelete)
        {
            account.SoftDelete();
            _dbContext.Users.Update(account);
        }
        else
        {
            _dbContext.Users.Remove(account);
        }
    }

    public async Task<List<string>> GetCountryRegionsAsync(string country)
    {
        var regions = await _dbContext
            .Addresses.Where(r => r.Country == country)
            .Select(r => r.RegionOrState)
            .Distinct() // This ensures unique values
            .ToListAsync();

        return regions;
    }
}
