using Application.Common.Exceptions;
using Application.Repositories;
using Application.Services;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.VeilingKlok;

public sealed record StartVeilingProductCommand(Guid KlokId, Guid ProductId) : IRequest;

public sealed class StartVeilingProductHandler : IRequestHandler<StartVeilingProductCommand>
{
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IProductRepository _productRepository;
    private readonly IVeilingKlokEngine _veilingKlokEngine;
    private readonly IUnitOfWork _unitOfWork;

    public StartVeilingProductHandler(
        IVeilingKlokRepository veilingKlokRepository,
        IProductRepository productRepository,
        IVeilingKlokEngine veilingKlokEngine,
        IUnitOfWork unitOfWork
    )
    {
        _veilingKlokRepository = veilingKlokRepository;
        _productRepository = productRepository;
        _veilingKlokEngine = veilingKlokEngine;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        StartVeilingProductCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Get the veiling klok
            var veilingKlok =
                await _veilingKlokRepository.GetByIdAsync(request.KlokId)
                ?? throw RepositoryException.NotFoundVeilingKlok();

            var products = (
                await _productRepository.GetAllByVeilingKlokIdAsync(request.KlokId)
            ).ToList();
            if (!products.Any(p => p.Id == request.ProductId))
                throw CustomException.InvalidVeilingKlokProductId();

            // Validate the product exists
            var product =
                await _productRepository.GetByIdAsync(request.ProductId)
                ?? throw RepositoryException.NotFoundProduct();

            // Verify the klok is active
            if (_veilingKlokEngine.IsVeillingRunning(request.KlokId))
                throw CustomException.CannotChangeRunningVeilingKlok();

            // Update the current product index in the klok
            var productIndex = products.FindIndex(p => p.Id == request.ProductId);
            veilingKlok.SetBiddingProductIndex(productIndex);
            veilingKlok.VeilingRounds++;

            // Update the engine to switch to the new product
            await _veilingKlokEngine.ChangeVeilingProductAsync(request.KlokId, request.ProductId);

            // Save changes
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
