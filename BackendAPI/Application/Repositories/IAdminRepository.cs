using Domain.Entities;

namespace Application.Repositories;

public interface IAdminRepository
{
    Task<Admin> CreateAsync(Admin admin);
    Task<Admin?> GetByIdAsync(Guid id);
    Task<Admin?> GetByEmailAsync(string email);
}
