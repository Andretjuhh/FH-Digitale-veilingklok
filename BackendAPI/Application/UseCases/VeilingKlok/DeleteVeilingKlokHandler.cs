using Application.Common.Exceptions;
using Application.Repositories;
using Application.Services;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.VeilingKlok;

public sealed record DeleteVeilingKlokCommand(Guid KlokId) : IRequest;

public sealed class DeleteVeilingKlokHandler : IRequestHandler<DeleteVeilingKlokCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IProductRepository _productRepository;

    public DeleteVeilingKlokHandler(
        IUnitOfWork unitOfWork,
        IVeilingKlokRepository veilingKlokRepository,
        IProductRepository productRepository
    )
    {
        _unitOfWork = unitOfWork;
        _veilingKlokRepository = veilingKlokRepository;
        _productRepository = productRepository;
    }

    public async Task Handle(DeleteVeilingKlokCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Get the VeilingKlok
            var veilingKlok = await _veilingKlokRepository.GetByIdAsync(request.KlokId)
                              ?? throw RepositoryException.NotFoundVeilingKlok();

            // Validate that the VeilingKlok is Scheduled (not started)
            if (veilingKlok.Status >= VeilingKlokStatus.Started)
                throw CustomException.CannotDeleteStartedVeilingKlok();

            // Get all products linked to this VeilingKlok
            var productIds = veilingKlok.GetOrderedProductIds();

            if (productIds.Any())
            {
                var products = await _productRepository.GetAllByIds(productIds);

                // Remove VeilingKlok reference from all products
                foreach (var product in products)
                {
                    product.RemoveVeilingKlok();
                    _productRepository.Update(product);
                }
            }

            // Hard delete the VeilingKlok (cascade will handle VeilingKlokProduct entries)
            _veilingKlokRepository.Delete(veilingKlok);

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