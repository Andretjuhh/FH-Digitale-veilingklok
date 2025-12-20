using Application.Common.Mappers;
using Application.Common.Models;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using MediatR;

namespace Application.UseCases.Product;

public sealed record CreateProductCommand(CreateProductDTO Payload, Guid KwekerId)
    : IRequest<ProductDetailsOutputDto>;

public sealed class CreateProductHandler
    : IRequestHandler<CreateProductCommand, ProductDetailsOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;

    // Dependency Injection of the Repository interface
    public CreateProductHandler(IUnitOfWork unitOfWork, IProductRepository productRepository)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
    }

    // The core method that executes the business logic
    public async Task<ProductDetailsOutputDto> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var dto = request.Payload;
            // Kweker only set the minimum price, stock and kwekerId
            var newProduct = new Domain.Entities.Product(dto.MinimumPrice, dto.Stock)
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = dto.ImageBase64,
                Dimension = dto.Dimension,
                KwekerId = request.KwekerId,
            };
            await _productRepository.AddAsync(newProduct);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return ProductMapper.ToOutputDto(newProduct, new KwekerInfo(Guid.Empty, ""));
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
