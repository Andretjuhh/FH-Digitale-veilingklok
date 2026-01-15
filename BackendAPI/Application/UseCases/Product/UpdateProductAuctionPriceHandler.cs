using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Product;

public sealed record UpdateProductAuctionPriceCommand(
    Guid ProductId,
    decimal AuctionPrice
) : IRequest<ProductDetailsOutputDto>;

public sealed class UpdateProductAuctionPriceHandler
    : IRequestHandler<UpdateProductAuctionPriceCommand, ProductDetailsOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IVeilingKlokRepository _veilingKlokRepository;

    public UpdateProductAuctionPriceHandler(IUnitOfWork unitOfWork, IProductRepository productRepository,
        IVeilingKlokRepository veilingKlokRepository)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _veilingKlokRepository = veilingKlokRepository;
    }

    public async Task<ProductDetailsOutputDto> Handle(
        UpdateProductAuctionPriceCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var (product, info) = await _productRepository.GetByIdWithKwekerIdAsync(request.ProductId)
                                  ?? throw RepositoryException.NotFoundProduct();

            // Check if product is linked to an active VeilingKlok
            if (product.VeilingKlokId.HasValue)
            {
                var veilingKlok = await _veilingKlokRepository.GetByIdAsync(product.VeilingKlokId.Value)
                                  ?? throw RepositoryException.NotFoundVeilingKlok();


                // Validate that the VeilingKlok hasn't been scheduled yet or is active
                if (veilingKlok.Status < VeilingKlokStatus.Ended)
                    throw CustomException.CannotUpdateProductLinkedToActiveVeilingKlok();

                // Update price in the VeilingKlok info
                veilingKlok.UpdateProductPrice(request.ProductId, request.AuctionPrice);
            }

            // Update the auction price & in the VeilingKlokProduct entity too
            product.UpdateAuctionPrice(request.AuctionPrice);

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return ProductMapper.ToOutputDto(product, info);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}