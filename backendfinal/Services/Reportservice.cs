using Microsoft.EntityFrameworkCore;
using backendfinal.Data;
using backendfinal.Dtos;
using backendfinal.Exceptions;

namespace backendfinal.Services;

public interface IReportService
{
    Task<InventoryReportResponse> GetInventoryAsync(int? productId);
    Task<OrderStatusResponse> GetOrderStatusAsync(int orderId);
}

public class ReportService(AppDbContext db) : IReportService
{
    // ── GET /inventory[?productId=] ───────────────────────────────────────────

    public async Task<InventoryReportResponse> GetInventoryAsync(int? productId)
    {
        // If productId is supplied, validate the item exists
        if (productId.HasValue)
        {
            var itemExists = await db.Items.AnyAsync(i => i.Id == productId.Value);
            if (!itemExists)
                throw new NotFoundException($"Item {productId.Value} not found.");
        }

        // BinLocations that have an item and quantity > 0
        var query = db.BinLocations
            .Include(bl => bl.Item)
            .Include(bl => bl.Bin)
            .Include(bl => bl.Shelf)
            .Where(bl => bl.ItemId != null && bl.Quantity > 0);

        if (productId.HasValue)
            query = query.Where(bl => bl.ItemId == productId.Value);

        var locations = await query.ToListAsync();

        var lines = locations.Select(bl => new InventoryLineDto
        {
            BinLocationId = bl.Id,
            BinId         = bl.BinId ?? 0,
            ItemId        = bl.ItemId ?? 0,
            ItemName      = bl.Item?.ItemName ?? "Unknown",
            Quantity      = bl.Quantity ?? 0,
            ShelfInfo     = bl.Shelf != null ? $"Shelf {bl.Shelf.Id}" : null
        }).ToList();

        return new InventoryReportResponse
        {
            Lines         = lines,
            TotalQuantity = lines.Sum(l => l.Quantity)
        };
    }

    // ── GET /orders/{id} ─────────────────────────────────────────────────────

    public async Task<OrderStatusResponse> GetOrderStatusAsync(int orderId)
    {
        var order = await db.CustomerOrders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
            .SingleOrDefaultAsync(o => o.Id == orderId)
            ?? throw new NotFoundException($"Order {orderId} not found.");

        var response = new OrderStatusResponse
        {
            OrderId      = order.Id,
            Status       = order.Status,
            ShippingFee  = order.ShippingFee,
            Date         = order.Date,
            CustomerId   = order.CustomerId,
            CustomerName = order.Customer?.Name ?? "Unknown",
            Items = order.OrderItems.Select(oi => new OrderStatusItemDto
            {
                ItemId   = oi.ItemId,
                ItemName = oi.Item?.ItemName ?? "Unknown",
                Qty      = oi.Qty,
                Price    = oi.Price
            }).ToList()
        };

        // Populate shipping info only when the order has been shipped
        if (order.Status == "SHIPPED")
        {
            response.ShippingInfo = new ShippingInfoDto
            {
                // Date is the last status-change timestamp — for shipped orders
                // this is the closest available timestamp without a separate audit table
                ShippedAt = order.Date
            };
        }

        return response;
    }
}