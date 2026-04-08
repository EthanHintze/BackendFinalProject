namespace backendfinal.Dtos;

public sealed record ReceiveShipmentItemResponse(int ProductId, int ExpectedQuantity, int ReceivedQuantity, int UpdatedInventoryQuantity);

public sealed record ReceiveShipmentResponse(int ShipmentId, DateTime? DateReceived, IReadOnlyList<ReceiveShipmentItemResponse> Items);
