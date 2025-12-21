using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Account;

public sealed record CreateAddressCommand(Guid AccountId, AddressInputDto Payload)
    : IRequest<AddressOutputDto>;

public sealed class CreateAddressHandler : IRequestHandler<CreateAddressCommand, AddressOutputDto>
{
    private readonly IKoperRepository _koperRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAddressHandler(IUnitOfWork unitOfWork, IKoperRepository koperRepository)
    {
        _koperRepository = koperRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AddressOutputDto> Handle(
        CreateAddressCommand request,
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
            var newAddress = new Address(
                dto.Street,
                dto.City,
                dto.RegionOrState,
                dto.PostalCode,
                dto.Country,
                koper.Id.ToString()
            );

            koper.AddNewAdress(newAddress, false);
            _koperRepository.Update(koper);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return AddressMapper.ToOutputDto(newAddress);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
