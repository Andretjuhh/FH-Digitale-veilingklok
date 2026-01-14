using Application.Common.Exceptions;
using Application.Repositories;
using Application.Services;
using MediatR;

namespace Application.UseCases.Account;

public sealed record ReactivateAccountCommand(Guid AccountId) : IRequest;

public sealed class ReactivateAccountHandler : IRequestHandler<ReactivateAccountCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public ReactivateAccountHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository
    )
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task Handle(
        ReactivateAccountCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            // Verify account exists
            var accountExists = await _userRepository.ExistingAccountAsync(request.AccountId);
            if (!accountExists)
                throw RepositoryException.NotFoundAccount();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Reactivate account
            await _userRepository.ReactivateAccountAsync(request.AccountId);

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
