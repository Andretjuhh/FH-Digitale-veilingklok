using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCases.Account;

public sealed record UpdateKwekerCommand(Guid AccountId, UpdateKwekerDTO Payload)
    : IRequest<AccountOutputDto>;

public sealed class UpdateKwekerHandler : IRequestHandler<UpdateKwekerCommand, AccountOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKwekerRepository _kwekerRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateKwekerHandler(
        IUnitOfWork unitOfWork,
        IKwekerRepository kwekerRepository,
        IPasswordHasher passwordHasher
    )
    {
        _unitOfWork = unitOfWork;
        _kwekerRepository = kwekerRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<AccountOutputDto> Handle(
        UpdateKwekerCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var kweker =
                await _kwekerRepository.GetKwekerByIdAsync(request.AccountId)
                ?? throw RepositoryException.NotFoundAccount();

            var dto = request.Payload;

            // Check if email is being changed and if it already exists
            if (dto.Email != null && kweker.Email != dto.Email)
            {
                if (await _kwekerRepository.ExistingAccountAsync(dto.Email))
                    throw RepositoryException.ExistingAccount();

                kweker.ChangeEmail(dto.Email);
            }

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(dto.Password)) kweker.ChangePassword(dto.Password, _passwordHasher);

            // Update Kweker-specific fields
            if (dto.CompanyName != null)
                kweker.CompanyName = dto.CompanyName;

            if (dto.FirstName != null)
                kweker.FirstName = dto.FirstName;

            if (dto.LastName != null)
                kweker.LastName = dto.LastName;

            if (dto.Telephone != null)
                kweker.Telephone = dto.Telephone;

            // Update address if provided
            if (dto.Address != null)
                kweker.Adress.UpdateAddress(
                    dto.Address.Street,
                    dto.Address.City,
                    dto.Address.RegionOrState,
                    dto.Address.PostalCode,
                    dto.Address.Country
                );

            _kwekerRepository.Update(kweker);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new AccountOutputDto
            {
                Email = kweker.Email,
                FirstName = kweker.FirstName,
                LastName = kweker.LastName,
                Telephone = kweker.Telephone,
                CompanyName = kweker.CompanyName,
                KvkNumber = kweker.KvkNumber,
                Address = AddressMapper.ToOutputDto(kweker.Adress),
                AccountType = kweker.AccountType
            };
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}