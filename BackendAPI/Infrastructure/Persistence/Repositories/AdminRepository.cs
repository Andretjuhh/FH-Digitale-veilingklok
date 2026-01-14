using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _dbContext;

    public AdminRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Admin> CreateAsync(Admin admin)
    {
        _dbContext.Admins.Add(admin);
        await _dbContext.SaveChangesAsync();
        return admin;
    }

    public async Task<Admin?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Admins.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Admin?> GetByEmailAsync(string email)
    {
        return await _dbContext.Admins.FirstOrDefaultAsync(a => a.Email == email);
    }
}
