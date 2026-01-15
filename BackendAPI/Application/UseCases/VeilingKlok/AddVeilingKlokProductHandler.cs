using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.Common.Models;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.VeilingKlok;

public sealed record AddProductToVeilingKlokCommand(
    Guid KlokId,
    Guid ProductId,
    decimal AuctionPrice
) : IRequest;

public sealed class AddVeilingKlokProductHandler
    : IRequestHandler<AddProductToVeilingKlokCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IProductRepository _productRepository;

    public AddVeilingKlokProductHandler(
        IUnitOfWork unitOfWork,
        IVeilingKlokRepository veilingKlokRepository,
        IProductRepository productRepository
    )
    {
        _unitOfWork = unitOfWork;
        _veilingKlokRepository = veilingKlokRepository;
        _productRepository = productRepository;
    }

    public async Task Handle(
        AddProductToVeilingKlokCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Get the VeilingKlok
            var veilingKlok = await _veilingKlokRepository.GetByIdAsync(request.KlokId)
                              ?? throw RepositoryException.NotFoundVeilingKlok();

            // Validate that the VeilingKlok hasn't started yet
            if (veilingKlok.Status >= VeilingKlokStatus.Started)
                throw CustomException.CannotUpdateProductLinkedToActiveVeilingKlok();

            // Get the product with kweker info
            var (product, kweker) = await _productRepository.GetByIdWithKwekerIdAsync(request.ProductId)
                                    ?? throw RepositoryException.NotFoundProduct();

            // Check if product is already linked to another VeilingKlok
            if (product.VeilingKlokId.HasValue && product.VeilingKlokId.Value != request.KlokId)
                throw CustomException.ProductAlreadyLinkedToVeilingKlok();

            // Check if product is already in this VeilingKlok
            if (product.VeilingKlokId.HasValue && product.VeilingKlokId.Value == request.KlokId)
                throw CustomException.ProductAlreadyInVeilingKlok();

            // Update the product auction price
            product.UpdateAuctionPrice(request.AuctionPrice);

            // Link product to VeilingKlok
            product.AddToVeilingKlok(request.KlokId);
            veilingKlok.AddProduct(request.ProductId, product.AuctionPrice ?? product.MinimumPrice);

            // Update repositories
            _productRepository.Update(product);
            _veilingKlokRepository.Update(veilingKlok);

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