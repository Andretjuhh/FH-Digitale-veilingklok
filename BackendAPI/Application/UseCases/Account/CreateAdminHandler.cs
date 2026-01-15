using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.DTOs.Input;
using Application.Services;
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
    private readonly IUnitOfWork _unitOfWork;

    public CreateAdminHandler(
        UserManager<Domain.Entities.Account> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IUnitOfWork unitOfWork
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
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

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var admin = new Domain.Entities.Account(dto.Email, AccountType.Admin);

            var result = await _userManager.CreateAsync(admin, dto.Password);
            result.ThrowIfFailed();

            var roleName = nameof(AccountType.Admin);
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
            await _userManager.AddToRoleAsync(admin, roleName);

            await _unitOfWork.CommitAsync(cancellationToken);

            return admin;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
