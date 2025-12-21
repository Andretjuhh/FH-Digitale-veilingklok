using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Account;

public sealed record UpdateKoperCommand(Guid AccountId, UpdateKoperDTO Payload)
    : IRequest<AccountOutputDto>;

public sealed class UpdateKoperHandler : IRequestHandler<UpdateKoperCommand, AccountOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKoperRepository _koperRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateKoperHandler(
        IUnitOfWork unitOfWork,
        IKoperRepository koperRepository,
        IPasswordHasher passwordHasher
    )
    {
        _unitOfWork = unitOfWork;
        _koperRepository = koperRepository;
        _passwordHasher = passwordHasher;
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

                koper.ChangeEmail(dto.Email);
            }

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(dto.Password)) koper.ChangePassword(dto.Password, _passwordHasher);

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
                AccountType = koper.AccountType
            };
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}