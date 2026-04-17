using Microsoft.EntityFrameworkCore;
using backendfinal.Data;
using backendfinal.Dtos;
using backendfinal.Entities;
using backendfinal.Exceptions;

namespace backendfinal.Services;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(CreateOrderRequest request);
    Task<PickOrderResponse> PickAsync(int orderId);
    Task<PackOrderResponse> PackAsync(int orderId);
    Task<ShipOrderResponse> ShipAsync(int orderId);
}

public class OrderService(AppDbContext db) : IOrderService
{
    // ── CREATE ────────────────────────────────────────────────────────────────

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request)
    {
        var customer = await db.Customers.FindAsync(request.CustomerId)
            ?? throw new NotFoundException($"Customer {request.CustomerId} not found.");

        var itemIds    = request.Items.Select(i => i.ItemId).Distinct().ToList();
        var foundItems = await db.Items
            .Where(i => itemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id);

        foreach (var id in itemIds)
            if (!foundItems.ContainsKey(id))
                throw new NotFoundException($"Item {id} not found.");

        var order = new CustomerOrder
        {
            CustomerId  = request.CustomerId,
            ShippingFee = request.ShippingFee,
            Date        = DateTime.UtcNow,
            Status      = "CREATED",
            OrderItems  = request.Items.Select(i => new CustomerOrderItem
            {
                ItemId = i.ItemId,
                Qty    = i.Qty,
                Price  = i.Price
            }).ToList()
        };

        db.CustomerOrders.Add(order);
        await db.SaveChangesAsync();

        return BuildOrderResponse(order, customer, foundItems);
    }

    // ── PICK ──────────────────────────────────────────────────────────────────

    public async Task<PickOrderResponse> PickAsync(int orderId)
    {
        await using var tx = await db.Database.BeginTransactionAsync();

        try
        {
            var order = await db.CustomerOrders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                .SingleOrDefaultAsync(o => o.Id == orderId)
                ?? throw new NotFoundException($"Order {orderId} not found.");

            // Idempotency: already picked — no inventory deduction
            if (order.Status == "PICKED")
                return new PickOrderResponse(order.Id, order.Status, "Order is already picked.", []);

            if (order.Status != "CREATED")
                throw new ConflictException(
                    $"Cannot pick an order in '{order.Status}' status. Order must be CREATED.");

            var pickedItems = new List<PickedItemDetail>();

            foreach (var lineItem in order.OrderItems)
            {
                // Find a single bin with sufficient stock — picks from one bin only
                var binLocation = await db.BinLocations
                    .Where(bl => bl.ItemId == lineItem.ItemId && bl.Quantity >= lineItem.Qty)
                    .FirstOrDefaultAsync()
                    ?? throw new BusinessRuleException(
                        $"Insufficient inventory for item {lineItem.ItemId} " +
                        $"({lineItem.Item.ItemName}). Required: {lineItem.Qty}.");

                binLocation.Quantity -= lineItem.Qty;

                pickedItems.Add(new PickedItemDetail(lineItem.ItemId, lineItem.Item.ItemName ?? string.Empty, lineItem.Qty, binLocation.BinId ?? 0));
            }

            order.Status = "PICKED";

            await db.SaveChangesAsync();
            await tx.CommitAsync();

            return new PickOrderResponse(order.Id, order.Status, "Order picked successfully.", pickedItems);
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    // ── PACK ──────────────────────────────────────────────────────────────────

    public async Task<PackOrderResponse> PackAsync(int orderId)
    {
        var order = await db.CustomerOrders.FindAsync(orderId)
            ?? throw new NotFoundException($"Order {orderId} not found.");

        if (order.Status == "PACKED")
            return new PackOrderResponse(order.Id, order.Status, "Order is already packed.");

        if (order.Status != "PICKED")
            throw new ConflictException(
                $"Cannot pack an order in '{order.Status}' status. Order must be PICKED.");

        order.Status = "PACKED";
        await db.SaveChangesAsync();

            return new PackOrderResponse(order.Id, order.Status, "Order packed successfully.");
    }

    // ── SHIP ──────────────────────────────────────────────────────────────────

    public async Task<ShipOrderResponse> ShipAsync(int orderId)
    {
        var order = await db.CustomerOrders.FindAsync(orderId)
            ?? throw new NotFoundException($"Order {orderId} not found.");

        if (order.Status == "SHIPPED")
            return new ShipOrderResponse(orderId, order.Status, "Order is already shipped.");

        if (order.Status != "PACKED")
            throw new ConflictException(
                $"Cannot ship an order in '{order.Status}' status. Order must be PACKED.");

        order.Status = "SHIPPED";
        await db.SaveChangesAsync();

        return new ShipOrderResponse(orderId, order.Status, "Order shipped successfully.");
    }

    // ── MAPPING ───────────────────────────────────────────────────────────────

    private static OrderResponse BuildOrderResponse(
        CustomerOrder order,
        Customer customer,
        Dictionary<int, Item> items) =>
        new(order.Id, order.CustomerId, customer.Name ?? string.Empty, order.Status, order.ShippingFee, order.Date, order.OrderItems.Select(oi => new OrderItemResponse(oi.Id, oi.ItemId, items[oi.ItemId].ItemName ?? string.Empty, oi.Qty, oi.Price)).ToList());
}