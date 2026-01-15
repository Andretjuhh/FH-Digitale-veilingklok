using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Order;

public sealed record CreateOrderCommand(Guid KoperId, CreateOrderDTO Payload)
    : IRequest<OrderOutputDto>;

public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IProductRepository _productRepository;
    private readonly IVeilingKlokEngine _veilingKlokEngine;
    private readonly ILogger<CreateOrderHandler> _logger;

    public CreateOrderHandler(
        IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        IVeilingKlokRepository veilingKlokRepository,
        IProductRepository productRepository,
        IVeilingKlokEngine veilingKlokEngine,
        ILogger<CreateOrderHandler> logger
    )
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _veilingKlokRepository = veilingKlokRepository;
        _productRepository = productRepository;
        _veilingKlokEngine = veilingKlokEngine;
        _logger = logger;
    }

    public async Task<OrderOutputDto> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        // Use this to get the current product price in the veilingklok engine
        var placedAt = new DateTimeOffset(DateTime.UtcNow);

        try
        {
            var dto = request.Payload;
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var veilingKlok =
                await _veilingKlokRepository.GetByIdAsync(dto.VeilingKlokId)
                ?? throw RepositoryException.NotFoundVeilingKlok();


            // Only allow orders when the auction clock is started and running ( meaning a running timer)
            if (!_veilingKlokEngine.IsVeillingRunning(veilingKlok.Id))
                throw CustomException.InvalidVeilingKlokStatus();

            if (!veilingKlok.GetOrderedProductIds().Contains(dto.ProductItemId))
                throw RepositoryException.NotFoundProduct();

            // Get the product from the DTO
            var product = await _productRepository.GetByIdAsync(dto.ProductItemId)
                          ?? throw RepositoryException.NotFoundProduct();

            // Check if there's enough stock
            if (product.Stock < dto.Quantity)
                throw CustomException.InsufficientStock();

            // Get the current price from the veiling klok engine at the time of order placement
            var currentPrice = await _veilingKlokEngine.GetCurrentVeilingPriceAsync(
                dto.VeilingKlokId,
                dto.ProductItemId,
                placedAt
            );

            _logger.LogInformation(
                "Current bidding price for Product {ProductId} in VeilingKlok {VeilingKlokId}: {CurrentPrice:C} at {PlacedAt}",
                dto.ProductItemId,
                dto.VeilingKlokId,
                currentPrice,
                placedAt
            );

            // Check if an order exists for this VeilingKlok + Koper + Product combination
            var order = await _orderRepository.FindOrderAsync(
                request.KoperId,
                dto.VeilingKlokId,
                dto.ProductItemId
            );

            // If no order exists, create one
            if (order == null)
            {
                order = new Domain.Entities.Order(request.KoperId)
                {
                    VeilingKlokId = veilingKlok.Id
                };
                await _orderRepository.AddAsync(order);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Before adding new order item, check if the order status is still open
            if (order.Status != OrderStatus.Open)
                throw CustomException.InvalidOperation();

            // Create the order item with the current price from the veiling klok engine
            var orderItem = new Domain.Entities.OrderItem(
                currentPrice,
                dto.Quantity,
                product,
                order.Id
            );

            // Add the order item to the order
            order.AddItem(orderItem);

            // Decrease the product stock
            product.DecreaseStock(dto.Quantity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            // Update the veiling klok engine with the placed bid
            await _veilingKlokEngine.PlaceVeilingBidAsync(
                order.VeilingKlokId,
                product.Id,
                placedAt,
                dto.Quantity
            );
            return OrderMapper.ToOutputDto(order);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}