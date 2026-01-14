using Application.Common.Exceptions;
using Application.DTOs.Input;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Account;

public sealed record CreateMeesterCommand(CreateMeesterDTO Payload) : IRequest<Guid>;

public sealed class CreateMeesterHandler : IRequestHandler<CreateMeesterCommand, Guid>
{
    private readonly UserManager<Domain.Entities.Account> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public CreateMeesterHandler(
        UserManager<Domain.Entities.Account> userManager,
        RoleManager<IdentityRole<Guid>> roleManager
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Guid> Handle(
        CreateMeesterCommand request,
        CancellationToken cancellationToken
    )
    {
        var dto = request.Payload;

        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
            throw RepositoryException.ExistingAccount();

        var meester = new Veilingmeester(dto.Email)
        {
            CountryCode = dto.CountryCode,
            Region = dto.Region,
            AuthorisatieCode = dto.AuthorisatieCode,
        };

        var result = await _userManager.CreateAsync(meester, dto.Password);

        if (!result.Succeeded)
        {
            throw new Exception(
                "Account creation failed: "
                    + string.Join(", ", result.Errors.Select(e => e.Description))
            );
        }

        var roleName = nameof(Domain.Enums.AccountType.Veilingmeester);
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }
        await _userManager.AddToRoleAsync(meester, roleName);

        return meester.Id;
    }
}
