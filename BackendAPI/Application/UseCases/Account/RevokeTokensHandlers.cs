using Application.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Account;

public sealed record RevokeTokensCommand(Guid AccountId) : IRequest;

public sealed class RevokeTokensHandler : IRequestHandler<RevokeTokensCommand>
{
    private readonly UserManager<Domain.Entities.Account> _userManager;

    public RevokeTokensHandler(UserManager<Domain.Entities.Account> userManager)
    {
        _userManager = userManager;
    }

    public async Task Handle(RevokeTokensCommand request, CancellationToken cancellationToken)
    {
        var account = await _userManager.FindByIdAsync(request.AccountId.ToString());
        if (account == null)
            throw RepositoryException.NotFoundAccount();

        await _userManager.UpdateSecurityStampAsync(account);
    }
}
