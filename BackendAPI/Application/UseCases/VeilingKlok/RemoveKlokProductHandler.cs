using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.Common.Models;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.VeilingKlok;

public sealed record RemoveProductFromVeilingKlokCommand(
    Guid KlokId,
    Guid ProductId
) : IRequest;

public sealed class RemoveKlokProductHandler
    : IRequestHandler<RemoveProductFromVeilingKlokCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IProductRepository _productRepository;

    public RemoveKlokProductHandler(
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
        RemoveProductFromVeilingKlokCommand request,
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

            // Get the product
            var product = await _productRepository.GetByIdAsync(request.ProductId)
                          ?? throw RepositoryException.NotFoundProduct();

            // Check if product is actually linked to this VeilingKlok
            if (product.VeilingKlokId != request.KlokId)
                throw CustomException.ProductNotLinkedToVeilingKlok();

            // Remove product from VeilingKlok
            product.RemoveVeilingKlok();
            veilingKlok.RemoveProductId(request.ProductId);

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