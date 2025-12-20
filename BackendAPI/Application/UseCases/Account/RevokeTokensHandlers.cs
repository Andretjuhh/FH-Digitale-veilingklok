using Application.Common.Exceptions;
using Application.Repositories;
using Application.Services;
using MediatR;

namespace Application.UseCases.Account;

public sealed record RevokeTokensCommand(Guid AccountId) : IRequest;

public sealed class RevokeTokensHandler : IRequestHandler<RevokeTokensCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public RevokeTokensHandler(IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task Handle(RevokeTokensCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var account = await _userRepository.GetByIdAsync(request.AccountId);
            if (account == null)
                throw RepositoryException.NotFoundAccount();

            // Logic to revoke all tokens for the account
            account.RevokeAllRefreshTokens();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}