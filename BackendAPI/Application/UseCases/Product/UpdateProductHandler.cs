using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Product;

public sealed record UpdateProductCommand(
    Guid ProductId,
    UpdateProductDTO Payload,
    Guid KwekerId
) : IRequest<ProductDetailsOutputDto>;

public sealed class UpdateProductHandler
    : IRequestHandler<UpdateProductCommand, ProductDetailsOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IVeilingKlokRepository _veilingKlokRepository;

    public UpdateProductHandler(IUnitOfWork unitOfWork, IProductRepository productRepository,
        IVeilingKlokRepository veilingKlokRepository)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _veilingKlokRepository = veilingKlokRepository;
    }

    public async Task<ProductDetailsOutputDto> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var dto = request.Payload;
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var (product, info) = await _productRepository.GetByIdWithKwekerIdAsync(request.ProductId, request.KwekerId)
                                  ?? throw RepositoryException.NotFoundProduct();

            // Check if product is linked to an active VeilingKlok
            if (product.VeilingKlokId.HasValue)
            {
                var veilingKlokStatus =
                    await _veilingKlokRepository.GetStatusAsync(product.VeilingKlokId.Value, cancellationToken);

                if (veilingKlokStatus < VeilingKlokStatus.Ended)
                    throw CustomException.CannotUpdateProductLinkedToActiveVeilingKlok();
            }

            product.Update(
                dto.Name,
                dto.Description,
                dto.ImageBase64,
                dto.Dimension,
                dto.MinimumPrice,
                dto.Stock
            );
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