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

public sealed record CreateAdminCommand(CreateAdminDTO Payload) : IRequest<AuthOutputDto>;

public sealed class CreateAdminHandler : IRequestHandler<CreateAdminCommand, AuthOutputDto>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public CreateAdminHandler(
        IAdminRepository adminRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService
    )
    {
        _adminRepository = adminRepository;
        _userRepository = userRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthOutputDto> Handle(
        CreateAdminCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var dto = request.Payload;
            if (await _userRepository.ExistingAccountAsync(dto.Email))
                throw RepositoryException.ExistingAccount();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Create Admin account
            var admin = new Admin(dto.Email, Password.Create(dto.Password, _passwordHasher));
            await _adminRepository.CreateAsync(admin);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate Tokens
            var (token, _) = _tokenService.GenerateAuthenticationTokens(admin);
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
