using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Account;

public sealed record GetAccountCommand(Guid AccountId, AccountType AccountType) : IRequest<object>;

public sealed class GetAccountHandler : IRequestHandler<GetAccountCommand, object>
{
    private readonly IKoperRepository _koperRepository;
    private readonly IKwekerRepository _kwekerRepository;
    private readonly IMeesterRepository _meesterRepository;
    private readonly IUserRepository _userRepository;

    public GetAccountHandler(
        IKoperRepository koperRepository,
        IKwekerRepository kwekerRepository,
        IMeesterRepository meesterRepository,
        IUserRepository userRepository
    )
    {
        _koperRepository = koperRepository;
        _kwekerRepository = kwekerRepository;
        _meesterRepository = meesterRepository;
        _userRepository = userRepository;
    }

    public async Task<object> Handle(GetAccountCommand request, CancellationToken cancellationToken)
    {
        var (accountId, accountType) = request;
        object account = accountType switch
        {
            AccountType.Koper => KoperMapper.ToOutputDto(
                await _koperRepository.GetKoperByIdAsync(accountId)
                    ?? throw RepositoryException.NotFoundAccount()
            ),
            AccountType.Kweker => KwekerMapper.ToOutputDto(
                await _kwekerRepository.GetKwekerByIdAsync(accountId)
                    ?? throw RepositoryException.NotFoundAccount()
            ),
            AccountType.Veilingmeester => VeilingmeesterMapper.ToOutputDto(
                await _meesterRepository.GetMeesterByIdAsync(accountId)
                    ?? throw RepositoryException.NotFoundAccount()
            ),
            AccountType.Admin => AdminMapper.ToOutputDto(
                await _userRepository.GetByIdAsync(accountId)
                    ?? throw RepositoryException.NotFoundAccount()
            ),
            _ => throw RepositoryException.NotFoundAccount(),
        };

        return account;
    }
}
