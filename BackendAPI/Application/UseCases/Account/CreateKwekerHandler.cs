using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.DTOs.Input;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Account;

public sealed record CreateKwekerCommand(CreateKwekerDTO Payload) : IRequest<Kweker>;

public sealed class CreateKwekerHandler : IRequestHandler<CreateKwekerCommand, Kweker>
{
    private readonly UserManager<Domain.Entities.Account> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IKwekerRepository _kwekerRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateKwekerHandler(
        UserManager<Domain.Entities.Account> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IKwekerRepository kwekerRepository,
        IAddressRepository addressRepository,
        IUnitOfWork unitOfWork
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _kwekerRepository = kwekerRepository;
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Kweker> Handle(
        CreateKwekerCommand request,
        CancellationToken cancellationToken
    )
    {
        var dto = request.Payload;

        if (await _kwekerRepository.ExistingKvkNumberAsync(dto.KvkNumber))
            throw RepositoryException.ExistingKvkNumber();

        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
            throw RepositoryException.ExistingAccount();

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Step 1: Create the Kweker account first (without address)
            var kweker = new Kweker(dto.Email)
            {
                CompanyName = dto.CompanyName,
                KvkNumber = dto.KvkNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Telephone = dto.Telephone,
            };

            var result = await _userManager.CreateAsync(kweker, dto.Password);
            result.ThrowIfFailed();

            // Step 2: Create the address with the AccountId
            var address = new Address(
                dto.Address.Street,
                dto.Address.City,
                dto.Address.RegionOrState,
                dto.Address.PostalCode,
                dto.Address.Country
            );

            // Set the AccountId manually using reflection
            typeof(Address).GetProperty(nameof(Address.AccountId))?.SetValue(address, kweker.Id);

            await _addressRepository.CreateAsync(address, cancellationToken);

            // Step 3: Set the address reference now that it has an ID
            typeof(Kweker).GetProperty(nameof(Kweker.AdressId))?.SetValue(kweker, address.Id);
            kweker.Adress = address;
            await _userManager.UpdateAsync(kweker);

            // Step 4: Assign role
            var roleName = nameof(Domain.Enums.AccountType.Kweker);
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
            await _userManager.AddToRoleAsync(kweker, roleName);

            await _unitOfWork.CommitAsync(cancellationToken);

            return kweker;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
