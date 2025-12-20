using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Order;

public sealed record CreateOrderCommand(CreateOrderDTO Payload, Guid VeilingKlokId)
    : IRequest<OrderOutputDto>;

public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;
    private readonly IVeilingKlokRepository _veilingKlokRepository;

    public CreateOrderHandler(
        IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        IVeilingKlokRepository veilingKlokRepository
    )
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _veilingKlokRepository = veilingKlokRepository;
    }

    public async Task<OrderOutputDto> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var dto = request.Payload;
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var veilingKlok = await _veilingKlokRepository.GetByIdAsync(request.VeilingKlokId) ??
                              throw RepositoryException.NotFoundVeilingKlok();

            // Only allow orders when the auction clock is started
            if (veilingKlok.Status > VeilingKlokStatus.Paused)
                throw CustomException.InvalidVeilingKlokStatus();

            // Create order for an clock to start auctioning products
            var order = new Domain.Entities.Order(dto.KoperId) { VeilingKlokId = veilingKlok.Id };

            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return OrderMapper.ToOutputDto(order);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}