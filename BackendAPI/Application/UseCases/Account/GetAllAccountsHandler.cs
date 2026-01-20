using Application.DTOs.Output;
using Application.Repositories;
using MediatR;

namespace Application.UseCases.Account;

public sealed record GetAllAccountsCommand() : IRequest<List<AccountListItemDto>>;

public sealed class GetAllAccountsHandler : IRequestHandler<GetAllAccountsCommand, List<AccountListItemDto>>
{
    private readonly IUserRepository _userRepository;

    public GetAllAccountsHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<AccountListItemDto>> Handle(
        GetAllAccountsCommand request,
        CancellationToken cancellationToken
    )
    {
        var accounts = await _userRepository.GetAllAccountsAsync();

        return accounts.Select(a => new AccountListItemDto
        {
            Id = a.Id,
            Email = a.Email,
            AccountType = a.AccountType,
            CreatedAt = a.CreatedAt,
            DeletedAt = a.DeletedAt
        }).ToList();
    }
}
