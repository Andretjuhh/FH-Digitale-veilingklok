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
) : IRequest<OrderItemOutputDto>;

public sealed class OrderProductHandler
    : IRequestHandler<CreateOrderProductCommand, OrderItemOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IVeilingKlokEngine _veilingKlokEngine;

    public OrderProductHandler(
        IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IVeilingKlokEngine veilingKlokEngine
    )
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _veilingKlokEngine = veilingKlokEngine;
    }

    public async Task<OrderItemOutputDto> Handle(
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
                await _orderRepository.GetWithKlokStatusByIdAsync(request.OrderId, request.KoperId) ??
                throw RepositoryException.NotFoundOrder();

            // Get the product and its kweker info
            var (product, info) = await _productRepository.GetByIdWithKwekerIdAsync(request.ProductId) ??
                                  throw RepositoryException.NotFoundProduct();

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

            var orderItem = new OrderItem(
                product.AuctionPrice.Value,
                request.Quantity,
                product,
                order.Id
            )
            {
                VeilingKlokId = order.VeilingKlokId
            };
            order.AddItem(orderItem);
            product.DecreaseStock(request.Quantity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Update the veiling klok engine with the new bid after saving the transaction
            await _veilingKlokEngine.PlaceVeilingBidAsync(
                order.VeilingKlokId,
                product.Id,
                orderPlacedAt,
                request.Quantity
            );

            await _unitOfWork.CommitAsync(cancellationToken);

            return OrderMapper.ItemOrder.ToOutputDto(
                orderItem,
                new OrderItemProduct(product.Name, product.Description, product.ImageUrl, info.CompanyName)
            );
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}