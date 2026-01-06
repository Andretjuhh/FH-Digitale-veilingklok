using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Output;
using Application.Repositories;
using MediatR;

namespace Application.UseCases.Product;

public sealed record GetProductDetailsCommand(Guid ProductId) : IRequest<ProductDetailsOutputDto>;

public sealed class GetProductDetailsHandler
    : IRequestHandler<GetProductDetailsCommand, ProductDetailsOutputDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductDetailsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDetailsOutputDto> Handle(
        GetProductDetailsCommand request,
        CancellationToken cancellationToken
    )
    {
        var (product, partialInfo) = await _productRepository.GetByIdWithKwekerIdAsync(request.ProductId) ??
                                     throw RepositoryException.NotFoundProduct();
        return ProductMapper.ToOutputDto(product, partialInfo);
    }
}