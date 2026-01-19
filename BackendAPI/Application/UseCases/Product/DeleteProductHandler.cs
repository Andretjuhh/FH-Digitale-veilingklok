using Application.Common.Exceptions;
using Application.Repositories;
using MediatR;

namespace Application.UseCases.Product;

public sealed record DeleteProductCommand(Guid ProductId, Guid KwekerId) : IRequest;

public sealed class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public DeleteProductHandler(
        IUnitOfWork unitOfWork,
        IProductRepository productRepository,
        IOrderRepository orderRepository
    )
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var product =
                await _productRepository.GetByIdAsync(request.ProductId, request.KwekerId)
                ?? throw RepositoryException.NotFoundProduct();

            // Check if product is attached to a veiling klok
            if (product.VeilingKlokId.HasValue)
            {
                throw CustomException.ProductAlreadyLinkedToVeilingKlok();
            }

            // Check if product has any orders
            if (await _orderRepository.HasOrdersAsync(product.Id))
            {
                throw CustomException.CannotDeleteProductWithOrders();
            }

            await _productRepository.DeleteAsync(product.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
