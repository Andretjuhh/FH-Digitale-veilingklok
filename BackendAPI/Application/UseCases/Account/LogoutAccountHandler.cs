using Application.Common.Exceptions;
using Application.Repositories;
using Application.Services;
using MediatR;

namespace Application.UseCases.Account;

public sealed record LogoutAccountCommand(Guid AccountId) : IRequest;

public sealed class LogoutAccountHandler : IRequestHandler<LogoutAccountCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public LogoutAccountHandler(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IUserRepository userRepository
    )
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    public async Task Handle(LogoutAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var account =
                await _userRepository.GetByIdAsync(request.AccountId)
                ?? throw RepositoryException.NotFoundAccount();

            var refreshToken = _tokenService.GetRefreshTokenFromCookies();
            if (refreshToken is not null)
            {
                account.RemoveRefreshToken(refreshToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}