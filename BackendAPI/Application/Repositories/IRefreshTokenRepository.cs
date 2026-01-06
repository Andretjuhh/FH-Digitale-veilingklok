using Domain.Entities;

namespace Application.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetById(string token);
    Task<RefreshToken?> GetByJtiAsync(Guid jti);
    Task AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task DeleteAsync(RefreshToken refreshToken);
    Task RevokeAllAsync(Guid accountId);
}