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

    public async Task<List<Account>> GetAllAccountsAsync()
    {
        return await _dbContext.Users.ToListAsync();
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
            // Hard delete: remove all related entities first in the correct order

            // 2. If this is a Veilingmeester, remove all VeilingKlok entities
            var veilingKlokken = await _dbContext
                .Veilingklokken.Where(vk => vk.VeilingmeesterId == id)
                .ToListAsync();

            if (veilingKlokken.Any())
                _dbContext.Veilingklokken.RemoveRange(veilingKlokken);

            // 3. If this is a Kweker, remove all products (and their related data)
            var products = await _dbContext.Products.Where(p => p.KwekerId == id).ToListAsync();

            if (products.Any())
            {
                // First remove order items that reference these products
                var productIds = products.Select(p => p.Id).ToList();
                var orderItems = await _dbContext
                    .OrderItems.Where(oi => productIds.Contains(oi.ProductId))
                    .ToListAsync();

                if (orderItems.Any())
                    _dbContext.OrderItems.RemoveRange(orderItems);

                // Then remove the products themselves
                _dbContext.Products.RemoveRange(products);
            }

            // 4. If this is a Koper, remove all orders and clear PrimaryAdressId
            var orders = await _dbContext
                .Orders.Include(o => o.OrderItems)
                .Where(o => o.KoperId == id)
                .ToListAsync();

            if (orders.Any())
            {
                foreach (var order in orders)
                    if (order.OrderItems.Any())
                        _dbContext.OrderItems.RemoveRange(order.OrderItems);

                _dbContext.Orders.RemoveRange(orders);
            }

            // Clear PrimaryAdressId if this is a Koper (to avoid foreign key constraint)
            var koper = await _dbContext.Kopers.FirstOrDefaultAsync(k => k.Id == id);
            if (koper != null && koper.PrimaryAdressId.HasValue)
            {
                typeof(Koper).GetProperty(nameof(Koper.PrimaryAdressId))?.SetValue(koper, null);
                _dbContext.Kopers.Update(koper);
                await _dbContext.SaveChangesAsync();
            }

            // 5. Remove addresses linked to this account
            var addresses = await _dbContext.Addresses.Where(a => a.AccountId == id).ToListAsync();

            if (addresses.Any())
                _dbContext.Addresses.RemoveRange(addresses);

            // 6. Finally, remove the account itself
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
