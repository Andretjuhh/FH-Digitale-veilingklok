using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.DTOs.Input;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Account;

public sealed record CreateKoperCommand(CreateKoperDTO Payload) : IRequest<Koper>;

public sealed class CreateKoperHandler : IRequestHandler<CreateKoperCommand, Koper>
{
    private readonly UserManager<Domain.Entities.Account> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IAddressRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateKoperHandler(
        UserManager<Domain.Entities.Account> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IAddressRepository addressRepository,
        IUnitOfWork unitOfWork
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Koper> Handle(CreateKoperCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Payload;

        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
            throw RepositoryException.ExistingAccount();

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var koper = new Koper(dto.Email)
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Telephone = dto.Telephone,
            };

            // Save Koper to get ID first
            var result = await _userManager.CreateAsync(koper, dto.Password);
            result.ThrowIfFailed();

            var address = new Address(
                dto.Address.Street,
                dto.Address.City,
                dto.Address.RegionOrState,
                dto.Address.PostalCode,
                dto.Address.Country
            );

            // Set AccountId on the address
            typeof(Address).GetProperty(nameof(Address.AccountId))?.SetValue(address, koper.Id);

            // Save address
            await _addressRepository.CreateAsync(address, cancellationToken);

            // Now set the primary address reference on the koper
            typeof(Koper).GetProperty(nameof(Koper.PrimaryAdressId))?.SetValue(koper, address.Id);

            // Add to Adresses collection using reflection since it's a readonly collection wrapper
            var adressesList = typeof(Koper).GetField(
                "IAdresses",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
            if (adressesList != null)
            {
                var list = (List<Address>)adressesList.GetValue(koper)!;
                if (!list.Contains(address))
                    list.Add(address);
            }

            // Update Koper with new Primary ID
            await _userManager.UpdateAsync(koper);

            var roleName = nameof(Domain.Enums.AccountType.Koper);
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
            await _userManager.AddToRoleAsync(koper, roleName);

            await _unitOfWork.CommitAsync(cancellationToken);

            return koper;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
