using Microsoft.EntityFrameworkCore;
using backendfinal.Data;
using backendfinal.Dtos;
using backendfinal.Entities;
using backendfinal.Exceptions;

namespace backendfinal.Services;

public class ShipmentService(AppDbContext db) : IShipmentService
{
    // ── CREATE ────────────────────────────────────────────────────────────────

    public async Task<ShipmentResponse> CreateShipmentAsync(CreateShipmentRequest request)
    {
        // Purchase order must exist
        var purchaseOrder = await db.PurchaseOrders
            .Include(po => po.OrderItems)
            .SingleOrDefaultAsync(po => po.Id == request.PurchaseOrderId)
            ?? throw new NotFoundException($"Purchase order {request.PurchaseOrderId} not found.");

        // Purchase order must have items
        if (!purchaseOrder.OrderItems.Any())
            throw new BusinessRuleException(
                $"Purchase order {request.PurchaseOrderId} has no items.");

        var shipment = new Shipment
        {
            OrderId      = request.PurchaseOrderId,
            HandlingCost = request.HandlingCost,
            Status       = "PENDING"
        };

        db.Shipments.Add(shipment);
        await db.SaveChangesAsync();

        return MapToResponse(shipment);
    }

    // ── RECEIVE ───────────────────────────────────────────────────────────────

    public async Task<ReceiveShipmentResponse> ReceiveShipmentAsync(ReceiveShipmentRequest request, CancellationToken cancellationToken = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var shipment = await db.Shipments
                .Include(s => s.ItemShipments)
                .Include(s => s.Order)
                    .ThenInclude(po => po.OrderItems)
                        .ThenInclude(poi => poi.Item)
                .SingleOrDefaultAsync(s => s.Id == request.ShipmentId, cancellationToken)
                ?? throw new NotFoundException($"Shipment {request.ShipmentId} not found.");

            // Idempotency: already received → 409, do not duplicate inventory
            if (shipment.Status == "RECEIVED" || shipment.DateReceived.HasValue)
                throw new ConflictException($"Shipment {request.ShipmentId} has already been received.");

            // Mark received
            shipment.Status       = "RECEIVED";
            shipment.DateReceived = DateTime.UtcNow;

            // Create ItemShipment records from PO items (increases inventory)
            // Only if none already exist (guards against partial receives)
            if (!shipment.ItemShipments.Any())
            {
                foreach (var poItem in shipment.Order.OrderItems)
                {
                    db.ItemShipments.Add(new ItemShipment
                    {
                        ShipmentId = shipment.Id,
                        ItemId     = poItem.ItemId,
                        Quantity   = poItem.Quantity,
                        ActionId   = 1 // Assuming 1 is receive action
                    });
                }
            }

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return new ReceiveShipmentResponse(
                ShipmentId: shipment.Id,
                DateReceived: shipment.DateReceived,
                Items: shipment.ItemShipments.Select(itemShipment => new ReceiveShipmentItemResponse(
                    ProductId: itemShipment.ItemId ?? 0,
                    ExpectedQuantity: shipment.Order.OrderItems.First(oi => oi.ItemId == itemShipment.ItemId).Quantity ?? 0,
                    ReceivedQuantity: itemShipment.Quantity ?? 0,
                    UpdatedInventoryQuantity: itemShipment.Quantity ?? 0 // Assuming inventory is increased by received quantity
                )).ToList()
            );
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    // ── MAPPING ───────────────────────────────────────────────────────────────

    private static ShipmentResponse MapToResponse(Shipment s) =>
        new()
        {
            Id              = s.Id,
            PurchaseOrderId = s.OrderId ?? 0,
            HandlingCost    = s.HandlingCost ?? 0,
            Status          = s.Status ?? "PENDING",
            DateReceived    = s.DateReceived,
            IsReceived      = s.DateReceived.HasValue
        };
}