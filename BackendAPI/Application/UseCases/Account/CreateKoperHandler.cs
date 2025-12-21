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

public sealed record CreateKoperCommand(CreateKoperDTO Payload) : IRequest<AuthOutputDto>;

public sealed class CreateKoperHandler : IRequestHandler<CreateKoperCommand, AuthOutputDto>
{
    private readonly IKoperRepository _koperRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public CreateKoperHandler(
        IKoperRepository koperRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService
    )
    {
        _koperRepository = koperRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthOutputDto> Handle(
        CreateKoperCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var dto = request.Payload;
            if (await _koperRepository.ExistingAccountAsync(dto.Email))
                throw RepositoryException.ExistingAccount();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var koper = new Koper(dto.Email, Password.Create(dto.Password, _passwordHasher))
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Telephone = dto.Telephone
            };
            var address = new Address(
                dto.Address.Street,
                dto.Address.City,
                dto.Address.RegionOrState,
                dto.Address.PostalCode,
                dto.Address.Country
            );
            koper.AddNewAdress(address, true);
            await _koperRepository.AddAsync(koper);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate Tokens
            var (token, _) = _tokenService.GenerateAuthenticationTokens(koper);
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