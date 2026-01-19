using Application.Common.Models;
using Domain.Entities;
using Domain.Enums;

namespace Application.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId);
    Task<Order?> GetByIdAsync(Guid orderId, Guid koperId);

    Task<(Order order, VeilingKlokStatus klokStatus)?> GetWithKlokStatusByIdAsync(
        Guid id,
        Guid koperId
    );

    Task<(Order order, List<OrderProductInfo> products)?> GetWithProductsByIdAsync(Guid id);

    Task<(Order order, List<OrderProductInfo> products)?> GetWithProductsByIdAsync(
        Guid id,
        Guid koperId
    );

    Task<(Order Order, List<OrderProductInfo> Products, KoperInfo Koper)?> GetKwekerOrderAsync(
        Guid orderId,
        Guid kwekerId
    );

    Task<(
        Order Order,
        List<OrderProductInfo> Products,
        KwekerInfo Kweker,
        KoperInfo Koper
    )?> GetKoperOrderAsync(Guid orderId, Guid koperId);

    Task<(Order Order, List<OrderProductInfo> Products, KoperInfo Koper)?> GetOrderDetailsAsync(
        Guid orderId
    );

    Task AddAsync(Order order);
    void Update(Order order);
    Task DeleteAsync(Order order);

    Task<Order?> FindOrderAsync(Guid koperId, Guid veilingKlokId, Guid? productId);

    Task<bool> HasOrdersAsync(Guid productId);

    Task<IEnumerable<Order>> GetAllByKoperIdAsync(Guid koperId);
    Task<IEnumerable<Order>> GetAllByIdsAsync(List<Guid> orderIds);

    Task<(
        IEnumerable<(Order order, List<OrderProductInfo> products)> Items,
        int TotalCount
    )> GetAllWithFilterAsync(
        OrderStatus? statusFilter,
        DateTime? beforeDate,
        DateTime? afterDate,
        Guid? productId,
        Guid? koperId,
        Guid? klokId,
        int pageNumber,
        int pageSize
    );

    Task<(
        IEnumerable<(Order Order, List<OrderProductInfo> Products, KoperInfo Koper)> Items,
        int TotalCount
    )> GetAllKwekerWithFilterAsync(
        string? ProductNameFilter,
        string? KoperNameFilter,
        OrderStatus? statusFilter,
        DateTime? beforeDate,
        DateTime? afterDate,
        Guid? productId,
        Guid kwekerId,
        int pageNumber,
        int pageSize
    );
}
