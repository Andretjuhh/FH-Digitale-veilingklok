using Application.Common.Models;
using Domain.Entities;
using Domain.Enums;

namespace Application.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId);
    Task<Order?> GetByIdAsync(Guid orderId, Guid koperId);
    Task<(Order order, VeilingKlokStatus klokStatus)?> GetWithKlokStatusByIdAsync(Guid id, Guid koperId);
    Task<(Order order, List<OrderProductInfo> products)?> GetWithProductsByIdAsync(Guid id);
    Task<(Order order, List<OrderProductInfo> products)?> GetWithProductsByIdAsync(Guid id, Guid koperId);

    Task<(Order Order, OrderProductInfo OProductInfo, KoperInfo Koper)?> GetKwekerOrderAsync(Guid orderId,
        Guid kwekerId);

    Task AddAsync(Order order);
    void Update(Order order);
    Task DeleteAsync(Order order);

    Task<IEnumerable<Order>> GetAllByKoperIdAsync(Guid koperId);
    Task<IEnumerable<Order>> GetAllByIdsAsync(List<Guid> orderIds);


    Task<(IEnumerable<Order> Items, int TotalCount)> GetAllWithFilterAsync(
        OrderStatus? statusFilter,
        DateTime? beforeDate,
        DateTime? afterDate,
        Guid? productId,
        Guid? koperId,
        Guid? klokId,
        int pageNumber,
        int pageSize
    );

    Task<(IEnumerable<(Order Order, OrderProductInfo Product, KoperInfo Koper)> Items, int TotalCount)>
        GetAllKwekerWithFilterAsync(
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