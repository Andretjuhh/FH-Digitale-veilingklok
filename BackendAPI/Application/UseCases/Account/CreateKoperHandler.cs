using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.DTOs.Input;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Account;

public sealed record CreateKoperCommand(CreateKoperDTO Payload) : IRequest<Koper>;

public sealed class CreateKoperHandler : IRequestHandler<CreateKoperCommand, Koper>
{
    private readonly UserManager<Domain.Entities.Account> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public CreateKoperHandler(
        UserManager<Domain.Entities.Account> userManager,
        RoleManager<IdentityRole<Guid>> roleManager
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Koper> Handle(CreateKoperCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Payload;

        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
            throw RepositoryException.ExistingAccount();

        var koper = new Koper(dto.Email)
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Telephone = dto.Telephone,
        };

        var address = new Address(
            dto.Address.Street,
            dto.Address.City,
            dto.Address.RegionOrState,
            dto.Address.PostalCode,
            dto.Address.Country
        );

        // Add address without setting as primary yet (Address doesn't have an ID until saved)
        koper.AddNewAdress(address, false);

        var result = await _userManager.CreateAsync(koper, dto.Password);

        result.ThrowIfFailed();

        // Now that the address has an ID, set it as primary
        koper.SetPrimaryAdress(address);
        await _userManager.UpdateAsync(koper);

        result.ThrowIfFailed();

        var roleName = nameof(Domain.Enums.AccountType.Koper);
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }
        await _userManager.AddToRoleAsync(koper, roleName);

        return koper;
    }
}
