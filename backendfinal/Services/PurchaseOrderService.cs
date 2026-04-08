using backendfinal.Data;
using backendfinal.Dtos;
using backendfinal.Entities;
using Microsoft.EntityFrameworkCore;

namespace backendfinal.Services;

public sealed class PurchaseOrderService : IPurchaseOrderService
{
    private readonly AppDbContext _dbContext;

    public PurchaseOrderService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PurchaseOrder> CreatePurchaseOrderAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default)
    {
        var purchaseOrder = new PurchaseOrder
        {
            DateOrdered = request.DateOrdered!.Value,
            Status = "CREATED"
        };

        foreach (var itemRequest in request.Items)
        {
            purchaseOrder.OrderItems.Add(new OrderItem
            {
                ItemId = itemRequest.ProductId,
                Quantity = itemRequest.Quantity
            });
        }

        _dbContext.PurchaseOrders.Add(purchaseOrder);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return purchaseOrder;
    }

    public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PurchaseOrders
            .Include(p => p.OrderItems)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
