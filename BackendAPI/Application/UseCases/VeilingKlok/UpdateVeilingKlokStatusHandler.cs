using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.VeilingKlok;

public sealed record UpdateVeilingKlokStatusCommand(Guid Id, VeilingKlokStatus Status) : IRequest;

public sealed class UpdateVeilingKlokStatusHandler : IRequestHandler<UpdateVeilingKlokStatusCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IProductRepository _productRepository;
    private readonly IVeilingKlokEngine _veilingKlokEngine;

    public UpdateVeilingKlokStatusHandler(
        IUnitOfWork unitOfWork,
        IVeilingKlokRepository veilingKlokRepository,
        IProductRepository productRepository,
        IVeilingKlokEngine veilingKlokEngine
    )
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _veilingKlokRepository = veilingKlokRepository;
        _veilingKlokEngine = veilingKlokEngine;
    }

    public async Task Handle(
        UpdateVeilingKlokStatusCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var veilingKlok =
                await _veilingKlokRepository.GetByIdAsync(request.Id)
                ?? throw RepositoryException.NotFoundVeilingKlok();

            var isRunning = _veilingKlokEngine.IsVeillingRunning(veilingKlok.Id);
            if (isRunning && request.Status == VeilingKlokStatus.Started)
                throw CustomException.CannotChangeRunningVeilingKlok();

            veilingKlok.UpdateStatus(request.Status);

            if (request.Status == VeilingKlokStatus.Started)
            {
                // Check if there is any active klok state in theat region status thats between started and paused
                var hasActiveVeiling = await _veilingKlokRepository.HasActiveVeilingInRegionAsync(
                    veilingKlok.RegionOrState,
                    veilingKlok.Country,
                    veilingKlok.Id,
                    cancellationToken
                );

                if (hasActiveVeiling)
                    throw CustomException.AlreadyActiveVeilingInRegion();

                var productIds = veilingKlok.GetOrderedProductIds();
                var products = await _productRepository.GetAllByIds(productIds);
                await _veilingKlokEngine.AddActiveVeilingKlokAsync(veilingKlok, products.ToList());
                await _veilingKlokEngine.StartVeilingAsync(veilingKlok.Id);
            }
            else if (request.Status == VeilingKlokStatus.Paused)
            {
                await _veilingKlokEngine.PauseVeilingAsync(veilingKlok.Id);
            }
            else if (request.Status == VeilingKlokStatus.Ended)
            {
                await _veilingKlokEngine.StopVeilingAsync(veilingKlok.Id);

                // When ending the veiling, we unlink the products from the active VeilingKlokId
                // The history is preserved in VeilingKlokProduct table
                var products = await _productRepository.GetAllByVeilingKlokIdAsync(veilingKlok.Id);
                foreach (var product in products)
                {
                    if (product.VeilingKlokId == veilingKlok.Id)
                    {
                        product.RemoveVeilingKlok();
                        _productRepository.Update(product);
                    }
                }
            }

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
