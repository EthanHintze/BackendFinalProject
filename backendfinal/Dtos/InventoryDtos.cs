namespace backendfinal.Dtos;

public record StoreInventoryRequest(int ItemId, int BinId, int Quantity);

public record StoreInventoryResponse(int BinLocationId, int BinId, int ItemId, string ItemName, int Quantity);