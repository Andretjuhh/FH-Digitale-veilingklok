using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using MediatR;

namespace Application.UseCases.Account;

public sealed record UpdatePrimaryAddressCommand(Guid AccountId, int AddressId)
    : IRequest<AddressOutputDto>;

public sealed class UpdatePrimaryAddressHandler
    : IRequestHandler<UpdatePrimaryAddressCommand, AddressOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKoperRepository _koperRepository;

    public UpdatePrimaryAddressHandler(IUnitOfWork unitOfWork, IKoperRepository koperRepository)
    {
        _unitOfWork = unitOfWork;
        _koperRepository = koperRepository;
    }

    public async Task<AddressOutputDto> Handle(
        UpdatePrimaryAddressCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var koper =
                await _koperRepository.GetKoperByIdAsync(request.AccountId)
                ?? throw RepositoryException.NotFoundAccount();
            var address =
                koper.Adresses.FirstOrDefault(a => a.Id == request.AddressId)
                ?? throw RepositoryException.NotFoundAddress();
            koper.SetPrimaryAdress(address);

            _koperRepository.Update(koper);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return AddressMapper.ToOutputDto(address);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}