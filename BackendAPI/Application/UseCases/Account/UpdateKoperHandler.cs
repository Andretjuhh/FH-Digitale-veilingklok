using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Account;

public sealed record UpdateKoperCommand(Guid AccountId, UpdateKoperDTO Payload)
    : IRequest<AccountOutputDto>;

public sealed class UpdateKoperHandler : IRequestHandler<UpdateKoperCommand, AccountOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKoperRepository _koperRepository;
    private readonly IPasswordHasher<Domain.Entities.Account> _passwordHasher;
    private readonly ILookupNormalizer _lookupNormalizer;

    public UpdateKoperHandler(
        IUnitOfWork unitOfWork,
        IKoperRepository koperRepository,
        IPasswordHasher<Domain.Entities.Account> passwordHasher,
        ILookupNormalizer lookupNormalizer
    )
    {
        _unitOfWork = unitOfWork;
        _koperRepository = koperRepository;
        _passwordHasher = passwordHasher;
        _lookupNormalizer = lookupNormalizer;
    }

    public async Task<AccountOutputDto> Handle(
        UpdateKoperCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var koper =
                await _koperRepository.GetKoperByIdAsync(request.AccountId)
                ?? throw RepositoryException.NotFoundAccount();

            var dto = request.Payload;

            // Check if email is being changed and if it already exists
            if (koper.Email != dto.Email)
            {
                if (await _koperRepository.ExistingAccountAsync(dto.Email))
                    throw RepositoryException.ExistingAccount();

                koper.Email = dto.Email;
                koper.UserName = dto.Email;
                koper.NormalizedEmail = _lookupNormalizer.NormalizeEmail(dto.Email);
                koper.NormalizedUserName = _lookupNormalizer.NormalizeName(dto.Email);
            }

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                koper.PasswordHash = _passwordHasher.HashPassword(koper, dto.Password);
            }

            // Update Koper-specific fields
            koper.FirstName = dto.FirstName;
            koper.LastName = dto.LastName;
            koper.Telephone = dto.Telephone;

            // Update address
            var primaryAddress = koper.Adresses.FirstOrDefault(a => a.Id == koper.PrimaryAdressId);
            if (primaryAddress != null)
            {
                primaryAddress.UpdateAddress(
                    dto.Address.Street,
                    dto.Address.City,
                    dto.Address.RegionOrState,
                    dto.Address.PostalCode,
                    dto.Address.Country
                );
            }
            else
            {
                // If no primary address exists, create a new one
                var newAddress = new Address(
                    dto.Address.Street,
                    dto.Address.City,
                    dto.Address.RegionOrState,
                    dto.Address.PostalCode,
                    dto.Address.Country
                );
                koper.AddNewAdress(newAddress, true);
            }

            _koperRepository.Update(koper);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new AccountOutputDto
            {
                Email = koper.Email,
                FirstName = koper.FirstName,
                LastName = koper.LastName,
                Telephone = koper.Telephone,
                AccountType = koper.AccountType,
            };
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
