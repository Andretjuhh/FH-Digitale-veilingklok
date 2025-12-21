using Application.Common.Exceptions;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using MediatR;

namespace Application.UseCases.Account;

public sealed record ReauthenticateTokenCommand() : IRequest<AuthOutputDto>;

public class ReauthenticateToken : IRequestHandler<ReauthenticateTokenCommand, AuthOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public ReauthenticateToken(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository
    )
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthOutputDto> Handle(
        ReauthenticateTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Get refresh token from cookie
            var cookie = _tokenService.GetContextRefreshToken() ?? throw CustomException.InvalidCredentials();

            // Validate refresh token
            var refreshToken = await _refreshTokenRepository.GetById(cookie);
            if (refreshToken == null || refreshToken.IsExpired || !refreshToken.IsActive)
            {
                _tokenService.ClearCookies();
                if (refreshToken != null)
                    await _refreshTokenRepository.DeleteAsync(refreshToken);
                throw CustomException.InvalidCredentials();
            }

            // Get account from the refresh token's AccountId
            var account =
                await _userRepository.GetByIdAsync(refreshToken.AccountId)
                ?? throw RepositoryException.ExistingAccount();

            // Generate new tokens
            var (accessToken, jti, expireAt) = _tokenService.GenerateAccessToken(account);

            // Update refresh token
            refreshToken.Jti = jti;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new AuthOutputDto
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = expireAt,
                AccountType = account.AccountType
            };
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}