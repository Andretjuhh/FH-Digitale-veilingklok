using Domain.Entities;

namespace Application.Repositories;

public interface IKwekerRepository : IUserRepository
{
    Task<bool> ExistingKvkNumberAsync(string kvkNumber);
    Task<Kweker?> GetKwekerByIdAsync(Guid accountId);
    Task AddAsync(Kweker kweker);
    void Update(Kweker kweker);
}