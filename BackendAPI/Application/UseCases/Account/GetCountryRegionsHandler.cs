using Application.Repositories;
using MediatR;

namespace Application.UseCases.Account;

public sealed record GetRegionsCommand(string Country) : IRequest<List<string>>;

public sealed class GetCountryRegionsHandler : IRequestHandler<GetRegionsCommand, List<string>>
{
    private readonly IUserRepository _userRepository;

    public GetCountryRegionsHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<string>> Handle(
        GetRegionsCommand request,
        CancellationToken cancellationToken
    )
    {
        return await _userRepository.GetCountryRegionsAsync(request.Country);
    }
}