using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.DTOs.Input;
using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Account;

public sealed record CreateMeesterCommand(CreateMeesterDTO Payload) : IRequest<Veilingmeester>;

public sealed class CreateMeesterHandler : IRequestHandler<CreateMeesterCommand, Veilingmeester>
{
    private readonly UserManager<Domain.Entities.Account> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMeesterHandler(
        UserManager<Domain.Entities.Account> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IUnitOfWork unitOfWork
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<Veilingmeester> Handle(
        CreateMeesterCommand request,
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
            var meester = new Veilingmeester(dto.Email)
            {
                CountryCode = dto.CountryCode,
                Region = dto.Region,
                AuthorisatieCode = dto.AuthorisatieCode,
            };

            var result = await _userManager.CreateAsync(meester, dto.Password);
            result.ThrowIfFailed();

            var roleName = nameof(Domain.Enums.AccountType.Veilingmeester);
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
            await _userManager.AddToRoleAsync(meester, roleName);

            await _unitOfWork.CommitAsync(cancellationToken);

            return meester;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
