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

public sealed record CreateMeesterCommand(CreateVeilingMeesterDTO Payload)
    : IRequest<AuthOutputDto>;

public sealed class CreateMeesterHandler : IRequestHandler<CreateMeesterCommand, AuthOutputDto>
{
    private readonly IMeesterRepository _meesterRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public CreateMeesterHandler(
        IMeesterRepository meesterRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService
    )
    {
        _unitOfWork = unitOfWork;
        _meesterRepository = meesterRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthOutputDto> Handle(
        CreateMeesterCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var dto = request.Payload;
            if (await _meesterRepository.ExistingAccountAsync(dto.Email))
                throw RepositoryException.ExistingAccount();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var meester = new Veilingmeester(
                dto.Email,
                Password.Create(dto.Password, _passwordHasher)
            )
            {
                CountryCode = dto.CountryCode,
                Region = dto.Region,
                AuthorisatieCode = dto.AuthorisatieCode
            };

            await _meesterRepository.AddAsync(meester);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate Tokens
            var (auth, _) = _tokenService.GenerateAuthenticationTokens(meester);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return auth;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}