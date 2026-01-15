using Domain.Entities;

namespace Application.Repositories;

public interface IAddressRepository
{
    Task<Address> CreateAsync(Address address, CancellationToken cancellationToken = default);
}
