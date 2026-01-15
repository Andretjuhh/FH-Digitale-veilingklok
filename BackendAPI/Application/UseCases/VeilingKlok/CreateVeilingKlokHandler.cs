using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.Common.Models;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using MediatR;

namespace Application.UseCases.VeilingKlok;

public sealed record CreateVeilingKlokCommand(CreateVeilingKlokDTO Payload, Guid MeesterId)
    : IRequest<VeilingKlokDetailsOutputDto>;

public sealed class CreateVeilingKlokHandler
    : IRequestHandler<CreateVeilingKlokCommand, VeilingKlokDetailsOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMeesterRepository _meesterRepository;
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IProductRepository _productRepository;

    public CreateVeilingKlokHandler(
        IUnitOfWork unitOfWork,
        IMeesterRepository meesterRepository,
        IVeilingKlokRepository veilingKlokRepository,
        IProductRepository productRepository
    )
    {
        _unitOfWork = unitOfWork;
        _veilingKlokRepository = veilingKlokRepository;
        _meesterRepository = meesterRepository;
        _productRepository = productRepository;
    }

    public async Task<VeilingKlokDetailsOutputDto> Handle(
        CreateVeilingKlokCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var dto = request.Payload;
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var meester = await _meesterRepository.GetMeesterByIdAsync(request.MeesterId) ??
                          throw RepositoryException.NotFoundAccount();

            //  Create new VeilingKlok
            var newKlok = new Domain.Entities.VeilingKlok
            {
                Country = meester.CountryCode,
                RegionOrState = meester.Region,
                ScheduledAt = dto.ScheduledAt,
                VeilingDurationSeconds = dto.VeilingDurationSeconds
            };
            newKlok.AssignVeilingmeester(request.MeesterId);

            // Persist the clock first so product FK updates won't violate constraints.
            await _veilingKlokRepository.AddAsync(newKlok);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Assign products to the VeilingKlok
            var productIds = dto.Products.Keys.ToList();
            var results = (await _productRepository.GetAllByIdsWithKwekerInfoAsync(productIds)).ToList();

            // Ensure all requested products were found
            if (results.Count != productIds.Count)
                throw RepositoryException.NotFoundProduct();

            // For each product set the auction price from DTO, then add to the veiling klok and update repository
            foreach (var result in results)
            {
                if (!dto.Products.TryGetValue(result.Product.Id, out var price))
                    throw RepositoryException.NotFoundProduct();

                // Use domain method to update the auction price (validates against minimum price)
                result.Product.UpdateAuctionPrice(price);

                // Mark as being auctioned on the veiling klok
                result.Product.AddToVeilingKlok(newKlok.Id);
                newKlok.AddProduct(result.Product.Id, result.Product.AuctionPrice ?? result.Product.MinimumPrice);

                // Persist change on product repository (unit of work will commit)
                _productRepository.Update(result.Product);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Map to output DTO (use Minimal mapper for list view)
            var productsDto = results.Select(r => ProductMapper.ToOutputDto(r.Product, r.Kweker)).ToList();
            var info = new VeilingKlokExtraInfo<ProductDetailsOutputDto>(0, productsDto);
            return VeilingKlokMapper.ToOutputDto(newKlok, info);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}