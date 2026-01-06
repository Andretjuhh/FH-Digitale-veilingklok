using Domain.Entities;

namespace Application.Repositories;

public interface IKoperRepository : IUserRepository
{
    Task<Koper?> GetKoperByIdAsync(Guid id);
    Task AddAsync(Koper koper);
    void Update(Koper koper);
}