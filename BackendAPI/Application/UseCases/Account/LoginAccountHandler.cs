using Application.Common.Exceptions;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCases.Account;

public sealed record LoginAccountCommand(RequestLoginDTO Payload) : IRequest<AuthOutputDto>;

public sealed class LoginAccountHandler : IRequestHandler<LoginAccountCommand, AuthOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginAccountHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher
    )
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthOutputDto> Handle(
        LoginAccountCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var dto = request.Payload;
            var account = await _userRepository.GetByEmailAsync(dto.Email);
            
            // Check if account exists first, but check if soft deleted before throwing invalid credentials
            if (account == null)
                throw CustomException.InvalidCredentials();
            
            // Check if account is soft deleted BEFORE checking password
            if (account.DeletedAt.HasValue)
                throw CustomException.AccountSoftDeleted();
            
            var isValid = account.Password.Verify(dto.Password, _passwordHasher);
            if (!isValid)
                throw CustomException.InvalidCredentials();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Generate Tokens
            var (authOutput, refreshToken) = _tokenService.GenerateAuthenticationTokens(account);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return authOutput;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}