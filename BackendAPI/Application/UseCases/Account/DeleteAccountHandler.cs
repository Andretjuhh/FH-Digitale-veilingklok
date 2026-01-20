using Application.Common.Exceptions;
using Application.DTOs.Input;
using Application.Repositories;
using Application.Services;
using MediatR;

namespace Application.UseCases.Account;

public sealed record DeleteAccountCommand(DeleteAccountRequestDTO Payload) : IRequest;

public sealed class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public DeleteAccountHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository
    )
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task Handle(
        DeleteAccountCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var dto = request.Payload;

            // Verify account exists
            var accountExists = await _userRepository.ExistingAccountAsync(dto.AccountId);
            if (!accountExists)
                throw RepositoryException.NotFoundAccount();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Delete account (soft or hard based on request)
            await _userRepository.DeleteAccountAsync(dto.AccountId, !dto.HardDelete);

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
