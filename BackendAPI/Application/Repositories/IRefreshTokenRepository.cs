using Domain.Entities;

namespace Application.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetById(Guid refreshTokenId);
    Task<RefreshToken?> GetByJtiAsync(string token);
    Task AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task DeleteAsync(RefreshToken refreshToken);
    Task RevokeAllAsync(Guid accountId);
}