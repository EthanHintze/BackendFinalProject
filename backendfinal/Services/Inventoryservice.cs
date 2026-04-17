using Microsoft.EntityFrameworkCore;
using backendfinal.Data;
using backendfinal.Dtos;
using backendfinal.Entities;
using backendfinal.Exceptions;

namespace backendfinal.Services;

public interface IInventoryService
{
    Task<StoreInventoryResponse> StoreAsync(StoreInventoryRequest request);
}

public class InventoryService(AppDbContext db) : IInventoryService
{
    public async Task<StoreInventoryResponse> StoreAsync(StoreInventoryRequest request)
    {
        await using var tx = await db.Database.BeginTransactionAsync();

        try
        {
            // Item must exist
            var item = await db.Items.FindAsync(request.ItemId)
                ?? throw new NotFoundException($"Item {request.ItemId} not found.");

            // Bin must exist
            var bin = await db.Bins
                .Include(b => b.BinLocations)
                .SingleOrDefaultAsync(b => b.Id == request.BinId)
                ?? throw new NotFoundException($"Bin {request.BinId} not found.");

            // A bin stores only one product — reject if occupied by a different item
            var existingLocation = bin.BinLocations.FirstOrDefault();
            if (existingLocation != null && existingLocation.ItemId != request.ItemId)
                throw new ConflictException(
                    $"Bin {request.BinId} already stores item {existingLocation.ItemId}. " +
                    "A bin can only hold one product.");

            // Item must have been received in at least one shipment
            var receivedQty = await db.ItemShipments
                .Where(ims => ims.ItemId == request.ItemId && ims.Shipment.DateReceived != null)
                .SumAsync(ims => (int?)ims.Quantity) ?? 0;

            if (receivedQty == 0)
                throw new BusinessRuleException(
                    $"Item {request.ItemId} has not been received in any shipment. " +
                    "Inventory must exist before it can be stored.");

            // Create or update BinLocation
            var location = existingLocation;

            if (location == null)
            {
                // BinLocation requires a ShelfId — find the first available shelf
                var shelf = await db.Shelves.FirstOrDefaultAsync()
                    ?? throw new BusinessRuleException(
                        "No shelves exist in the warehouse. Cannot store inventory.");

                location = new BinLocation
                {
                    BinId    = request.BinId,
                    ShelfId  = shelf.Id,
                    ItemId   = request.ItemId,
                    Quantity = 0
                };
                db.BinLocations.Add(location);
            }

            location.ItemId    = request.ItemId;
            location.Quantity += request.Quantity;

            if (location.Quantity < 0)
                throw new BusinessRuleException("Inventory cannot be negative.");

            await db.SaveChangesAsync();
            await tx.CommitAsync();

            return new StoreInventoryResponse(location.Id, request.BinId, request.ItemId, item.ItemName ?? string.Empty, location.Quantity ?? 0);
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}