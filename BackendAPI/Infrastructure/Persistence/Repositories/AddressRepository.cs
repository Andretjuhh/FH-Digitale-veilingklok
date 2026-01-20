using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;

namespace Infrastructure.Persistence.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly AppDbContext _dbContext;

    public AddressRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Address> CreateAsync(
        Address address,
        CancellationToken cancellationToken = default
    )
    {
        _dbContext.Addresses.Add(address);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return address;
    }
}
