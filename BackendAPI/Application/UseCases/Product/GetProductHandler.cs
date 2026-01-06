using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Output;
using Application.Repositories;
using MediatR;

namespace Application.UseCases.Product;

public sealed record GetProductCommand(Guid ProductId) : IRequest<ProductOutputDto>;

public sealed class GetProductHandler : IRequestHandler<GetProductCommand, ProductOutputDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductOutputDto> Handle(
        GetProductCommand request,
        CancellationToken cancellationToken
    )
    {
        var (product, info) = await _productRepository.GetByIdWithKwekerIdAsync(request.ProductId) ??
                              throw RepositoryException.NotFoundProduct();
        return ProductMapper.Minimal.ToOutputDto(product, info);
    }
}