using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Account;

public sealed record LogoutAccountCommand(Guid AccountId) : IRequest;

public sealed class LogoutAccountHandler : IRequestHandler<LogoutAccountCommand>
{
    private readonly UserManager<Domain.Entities.Account> _userManager;

    public LogoutAccountHandler(UserManager<Domain.Entities.Account> userManager)
    {
        _userManager = userManager;
    }

    public async Task Handle(LogoutAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.AccountId.ToString());
        if (user != null)
        {
            await _userManager.UpdateSecurityStampAsync(user);
        }
    }
}
