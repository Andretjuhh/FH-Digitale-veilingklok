using Application.Common.Exceptions;
using Application.DTOs.Input;
using Application.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Account;

public sealed record CreateKwekerCommand(CreateKwekerDTO Payload) : IRequest<Guid>;

public sealed class CreateKwekerHandler : IRequestHandler<CreateKwekerCommand, Guid>
{
    private readonly UserManager<Domain.Entities.Account> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IKwekerRepository _kwekerRepository;

    public CreateKwekerHandler(
        UserManager<Domain.Entities.Account> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IKwekerRepository kwekerRepository
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _kwekerRepository = kwekerRepository;
    }

    public async Task<Guid> Handle(CreateKwekerCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Payload;

        if (await _kwekerRepository.ExistingKvkNumberAsync(dto.KvkNumber))
            throw RepositoryException.ExistingKvkNumber();

        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
            throw RepositoryException.ExistingAccount();

        var kweker = new Kweker(dto.Email)
        {
            CompanyName = dto.CompanyName,
            KvkNumber = dto.KvkNumber,
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

        kweker.UpdateAdress(address);

        var result = await _userManager.CreateAsync(kweker, dto.Password);

        if (!result.Succeeded)
        {
            throw new Exception(
                "Account creation failed: "
                    + string.Join(", ", result.Errors.Select(e => e.Description))
            );
        }

        var roleName = nameof(Domain.Enums.AccountType.Kweker);
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }
        await _userManager.AddToRoleAsync(kweker, roleName);

        return kweker.Id;
    }
}
