using backendfinal.Dtos;
using backendfinal.Entities;

namespace backendfinal.Services;

public interface IPurchaseOrderService
{
    Task<PurchaseOrder> CreatePurchaseOrderAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default);
    Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id, CancellationToken cancellationToken = default);
}
