using backendfinal.Data;
using backendfinal.Dtos;
using backendfinal.Entities;
using Microsoft.EntityFrameworkCore;

namespace backendfinal.Services;

public sealed class ShipmentService : IShipmentService
{
    private readonly AppDbContext _dbContext;

    public ShipmentService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ReceiveShipmentResponse> ReceiveShipmentAsync(ReceiveShipmentRequest request, CancellationToken cancellationToken = default)
    {
        var shipment = await _dbContext.Shipments
            .Include(s => s.ItemShipments)
            .FirstOrDefaultAsync(s => s.Id == request.ShipmentId, cancellationToken);

        if (shipment is null)
        {
            throw new KeyNotFoundException("Shipment not found.");
        }

        var expectedMap = shipment.ItemShipments
            .Where(i => i.ItemId.HasValue)
            .ToDictionary(i => i.ItemId!.Value, i => i);

        var receivedItems = request.Items;

        if (receivedItems.Count != expectedMap.Count || receivedItems.Any(i => !expectedMap.ContainsKey(i.ProductId)))
        {
            throw new ShipmentValidationException("Shipment items do not match expected items.");
        }

        var responseItems = new List<ReceiveShipmentItemResponse>(receivedItems.Count);

        foreach (var item in receivedItems)
        {
            if (item.Quantity <= 0)
            {
                throw new ShipmentValidationException("Quantity must be greater than 0.");
            }

            var expectedShipmentItem = expectedMap[item.ProductId];
            var expectedQuantity = expectedShipmentItem.Quantity ?? 0;
            var quantityReceived = item.Quantity;

            var inventoryLocation = await _dbContext.BinLocations
                .FirstOrDefaultAsync(b => b.ItemId == item.ProductId, cancellationToken);

            if (inventoryLocation is null)
            {
                inventoryLocation = new BinLocation
                {
                    ItemId = item.ProductId,
                    Quantity = quantityReceived
                };

                _dbContext.BinLocations.Add(inventoryLocation);
            }
            else
            {
                inventoryLocation.Quantity = (inventoryLocation.Quantity ?? 0) + quantityReceived;
            }

            expectedShipmentItem.Quantity = quantityReceived;

            responseItems.Add(new ReceiveShipmentItemResponse(
                item.ProductId,
                expectedQuantity,
                quantityReceived,
                inventoryLocation.Quantity ?? quantityReceived));
        }

        shipment.DateReceived = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ReceiveShipmentResponse(shipment.Id, shipment.DateReceived, responseItems);
    }
}

internal sealed class ShipmentValidationException : Exception
{
    public ShipmentValidationException(string message)
        : base(message)
    {
    }
}
