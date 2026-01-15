using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.DTOs.Input;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Account;

public sealed record CreateAdminCommand(CreateAdminDTO Payload) : IRequest<Domain.Entities.Account>;

public sealed class CreateAdminHandler
    : IRequestHandler<CreateAdminCommand, Domain.Entities.Account>
{
    private readonly UserManager<Domain.Entities.Account> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public CreateAdminHandler(
        UserManager<Domain.Entities.Account> userManager,
        RoleManager<IdentityRole<Guid>> roleManager
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Domain.Entities.Account> Handle(
        CreateAdminCommand request,
        CancellationToken cancellationToken
    )
    {
        var dto = request.Payload;

        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
            throw RepositoryException.ExistingAccount();

        var admin = new Domain.Entities.Account(dto.Email, AccountType.Admin);

        var result = await _userManager.CreateAsync(admin, dto.Password);

        result.ThrowIfFailed();

        var roleName = nameof(AccountType.Admin);
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }
        await _userManager.AddToRoleAsync(admin, roleName);

        return admin;
    }
}
