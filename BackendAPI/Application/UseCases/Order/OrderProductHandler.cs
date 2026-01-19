using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.Common.Models;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;

namespace Application.UseCases.Order;

public sealed record CreateOrderProductCommand(
    Guid KoperId,
    Guid OrderId,
    Guid ProductId,
    int Quantity
) : IRequest<OrderProductOutputDto>;

public sealed class OrderProductHandler
    : IRequestHandler<CreateOrderProductCommand, OrderProductOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IVeilingKlokEngine _veilingKlokEngine;

    public OrderProductHandler(
        IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IVeilingKlokRepository veilingKlokRepository,
        IVeilingKlokEngine veilingKlokEngine
    )
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _veilingKlokRepository = veilingKlokRepository;
        _veilingKlokEngine = veilingKlokEngine;
    }

    public async Task<OrderProductOutputDto> Handle(
        CreateOrderProductCommand request,
        CancellationToken cancellationToken
    )
    {
        // Record the time the order was placed
        var orderPlacedAt = DateTimeOffset.UtcNow;

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var (order, klokStatus) =
                await _orderRepository.GetWithKlokStatusByIdAsync(request.OrderId, request.KoperId)
                ?? throw RepositoryException.NotFoundOrder();

            var veilingKlok =
                await _veilingKlokRepository.GetByIdAsync(order.VeilingKlokId)
                ?? throw RepositoryException.NotFoundVeilingKlok();

            // Get the product and its kweker info
            var (product, info) =
                await _productRepository.GetByIdWithKwekerIdAsync(request.ProductId)
                ?? throw RepositoryException.NotFoundProduct();

            // Ensure the veiling klok is running
            if (!_veilingKlokEngine.IsVeillingRunning(order.VeilingKlokId))
                throw CustomException.InvalidOperation();

            // If order is not started and order is at scheduled status, we cannot add products
            if (klokStatus > VeilingKlokStatus.Started || VeilingKlokStatus.Scheduled == klokStatus)
                throw CustomException.InvalidOperation();

            if (!product.AuctionPrice.HasValue)
                throw ProductValidationException.ProductNotAvailableForAuction();

            if (product.Auctioned == false)
                throw CustomException.InvalidOperation();

            if (product.Stock < request.Quantity)
                throw CustomException.InsufficientStock();

            var currentPrice = _veilingKlokEngine.GetKlokCurrentTickedPrice(
                order.VeilingKlokId,
                orderPlacedAt
            );

            var orderItem = new OrderItem(currentPrice, request.Quantity, product, order.Id);
            order.AddItem(orderItem);
            product.DecreaseStock(request.Quantity);

            // If stock becomes 0, pause the auction clock
            if (product.Stock == 0)
            {
                veilingKlok.UpdateStatus(VeilingKlokStatus.Paused);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Update the veiling klok engine with the new bid after saving the transaction
            await _veilingKlokEngine.PlaceVeilingBidAsync(
                order.VeilingKlokId,
                product.Id,
                orderPlacedAt,
                request.Quantity
            );

            // If stock was 0, call the pause in engine too
            if (product.Stock == 0)
            {
                await _veilingKlokEngine.PauseVeilingAsync(veilingKlok.Id);
            }

            return OrderMapper.ItemOrder.ToOutputDto(
                orderItem,
                new OrderItemProduct(
                    product.Name,
                    product.Description,
                    product.ImageUrl,
                    info.CompanyName
                )
            );
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
