using Domain.Entities;

namespace Application.Repositories;

public interface IKwekerRepository : IUserRepository
{
    Task<Kweker?> GetKwekerByIdAsync(Guid accountId);
    Task AddAsync(Kweker kweker);
    void Update(Kweker kweker);
}