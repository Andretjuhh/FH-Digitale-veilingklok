using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _dbContext;

    public RefreshTokenRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefreshToken?> GetById(string token)
    {
        return await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<RefreshToken?> GetByJtiAsync(Guid jti)
    {
        return await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Jti == jti);
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken);
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        _dbContext.RefreshTokens.Update(refreshToken);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(RefreshToken refreshToken)
    {
        _dbContext.RefreshTokens.Remove(refreshToken);
        await Task.CompletedTask;
    }

    public async Task RevokeAllAsync(Guid accountId)
    {
        var tokens = await _dbContext.RefreshTokens
            .Where(rt => rt.AccountId == accountId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens) token.RevokedAt = DateTimeOffset.UtcNow;

        _dbContext.RefreshTokens.UpdateRange(tokens);
    }
}