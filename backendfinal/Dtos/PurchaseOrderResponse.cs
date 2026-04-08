namespace backendfinal.Dtos;

public sealed record PurchaseOrderItemResponse(int ProductId, int Quantity);

public sealed record PurchaseOrderResponse(int Id, DateTime? DateOrdered, string? Status, IReadOnlyList<PurchaseOrderItemResponse> Items);
