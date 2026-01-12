using Application.Common.Models;
using Domain.Entities;

namespace Application.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product);
    void Update(Product product);
    Task DeleteAsync(Guid productId);

    Task<Product?> GetByIdAsync(Guid productId);
    Task<Product?> GetByIdAsync(Guid productId, Guid kwekerId);
    Task<(Product Product, KwekerInfo Kweker)?> GetByIdWithKwekerIdAsync(Guid productId);
    Task<(Product Product, KwekerInfo Kweker)?> GetByIdWithKwekerIdAsync(Guid productId, Guid kwekerId);

    Task<IEnumerable<Product>> GetAllByIds(List<Guid> productIds);
    Task<IEnumerable<Product>> GetAllByVeilingKlokIdAsync(Guid veilingKlokId);
    Task<IEnumerable<(Product Product, KwekerInfo Kweker)>> GetAllByIdsWithKwekerInfoAsync(List<Guid> ids);

    Task<(
        IEnumerable<(Product Product, KwekerInfo Kweker)> Items,
        int TotalCount
        )> GetAllWithFilterAsync(
        string? nameFilter,
        string? regionFilter,
        decimal? maxPrice,
        Guid? kwekerId,
        int pageNumber,
        int pageSize
    );
}