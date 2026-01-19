using System.Linq;
using Application.Common.Exceptions;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Account;

public sealed record ReactivateAccountCommand(Guid AccountId) : IRequest;

public sealed class ReactivateAccountHandler : IRequestHandler<ReactivateAccountCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<Domain.Entities.Account> _userManager;

    public ReactivateAccountHandler(
        IUnitOfWork unitOfWork,
        UserManager<Domain.Entities.Account> userManager
    )
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task Handle(ReactivateAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.AccountId.ToString());
            if (user == null)
                throw RepositoryException.NotFoundAccount();

            if (!user.DeletedAt.HasValue)
                return;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Reactivate account
            user.Reactivate();

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                // Optionally handle errors from Identity
                throw new Exception(
                    "Failed to reactivate account: "
                        + string.Join(", ", result.Errors.Select(e => e.Description))
                );
            }

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
