using Application.Common.Exceptions;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCases.Account;

public sealed record UpdateVeilingmeesterCommand(Guid AccountId, UpdateVeilingMeesterDTO Payload)
    : IRequest<AccountOutputDto>;

public sealed class UpdateVeilingmeesterHandler
    : IRequestHandler<UpdateVeilingmeesterCommand, AccountOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMeesterRepository _meesterRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateVeilingmeesterHandler(
        IUnitOfWork unitOfWork,
        IMeesterRepository meesterRepository,
        IPasswordHasher passwordHasher
    )
    {
        _unitOfWork = unitOfWork;
        _meesterRepository = meesterRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<AccountOutputDto> Handle(
        UpdateVeilingmeesterCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var meester =
                await _meesterRepository.GetMeesterByIdAsync(request.AccountId)
                ?? throw RepositoryException.NotFoundAccount();

            var dto = request.Payload;

            // Check if email is being changed and if it already exists
            if (dto.Email != null && meester.Email != dto.Email)
            {
                if (await _meesterRepository.ExistingAccountAsync(dto.Email))
                    throw RepositoryException.ExistingAccount();

                meester.ChangeEmail(dto.Email);
            }

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(dto.Password)) meester.ChangePassword(dto.Password, _passwordHasher);

            // Update Veilingmeester-specific fields
            if (dto.Regio != null)
                meester.Region = dto.Regio;

            if (dto.AuthorisatieCode != null)
                meester.AuthorisatieCode = dto.AuthorisatieCode;

            _meesterRepository.Update(meester);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new AccountOutputDto
            {
                Email = meester.Email,
                AccountType = meester.AccountType,
                CountryCode = meester.CountryCode,
                Region = meester.Region
            };
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}