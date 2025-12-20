using Application.Common.Exceptions;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Account;

public sealed record CreateKwekerCommand(CreateKwekerDTO Payload) : IRequest<AuthOutputDto>;

public sealed class CreateKwekerHandler : IRequestHandler<CreateKwekerCommand, AuthOutputDto>
{
    private readonly IKwekerRepository _kwekerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public CreateKwekerHandler(
        IKwekerRepository kwekerRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService
    )
    {
        _kwekerRepository = kwekerRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthOutputDto> Handle(
        CreateKwekerCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var dto = request.Payload;
            if (await _kwekerRepository.ExistingAccountAsync(dto.Email))
                throw RepositoryException.ExistingAccount();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var kweker = new Kweker(dto.Email, Password.Create(dto.Password, _passwordHasher))
            {
                CompanyName = dto.CompanyName,
                KvkNumber = dto.KvkNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Telephone = dto.Telephone,
                Adress = new Address(
                    dto.Address.Street,
                    dto.Address.City,
                    dto.Address.RegionOrState,
                    dto.Address.PostalCode,
                    dto.Address.Country
                )
            };
            await _kwekerRepository.AddAsync(kweker);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate Tokens
            var token = _tokenService.GenerateAuthenticationTokens(kweker);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return token;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}