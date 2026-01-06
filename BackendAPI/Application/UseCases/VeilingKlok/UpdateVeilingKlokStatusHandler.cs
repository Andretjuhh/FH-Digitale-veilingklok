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
            var veilingKlok = await _veilingKlokRepository.GetByIdAsync(request.Id) ??
                              throw RepositoryException.NotFoundVeilingKlok();

            // Check if a veiling is running
            if (_veilingKlokEngine.IsVeillingRunning(veilingKlok.Id))
                throw CustomException.CannotChangeRunningVeilingKlok();

            veilingKlok.UpdateStatus(request.Status);
            if (request.Status == VeilingKlokStatus.Started)
            {
                var product = await _productRepository.GetAllByIds(veilingKlok.ProductsIds.ToList());
                await _veilingKlokEngine.AddActiveVeilingKlokAsync(veilingKlok, product.ToList());
                await _veilingKlokEngine.StartVeilingAsync(veilingKlok.Id);
            }
            else if (request.Status == VeilingKlokStatus.Stopped)
            {
                await _veilingKlokEngine.StopVeilingAsync(veilingKlok.Id);
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