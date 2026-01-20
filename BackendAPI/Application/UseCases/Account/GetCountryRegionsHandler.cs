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
        // Only Netherlands is supported for now, so we return a hardcoded list
        // return await _userRepository.GetCountryRegionsAsync(request.Country);
        var regions = new List<string>
        {
            "Noord-Holland",
            "Zuid-Holland",
            "Utrecht",
            "Noord-Brabant",
            "Gelderland",
            "Limburg",
            "Zeeland",
            "Flevoland",
            "Friesland",
            "Groningen",
            "Drenthe",
            "Overijssel"
        };

        return await Task.FromResult(regions);
    }
}