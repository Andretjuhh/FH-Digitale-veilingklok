using Domain.Entities;

namespace Application.Repositories;

public interface IUserRepository
{
    Task<bool> ExistingAccountAsync(string email);
    Task<bool> ExistingAccountAsync(Guid accountId);
    Task<Account?> GetByEmailAsync(string email);
    Task<Account?> GetByIdAsync(Guid accountId);
    Task DeleteAccountAsync(Guid id, bool softDelete = true);
    Task<List<string>> GetCountryRegionsAsync(string country);
}