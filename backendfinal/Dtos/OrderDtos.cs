namespace backendfinal.Dtos;

public record CreateOrderRequest(int CustomerId, decimal ShippingFee, List<OrderItemRequest> Items);

public record OrderItemRequest(int ItemId, int Qty, decimal Price);

public record OrderResponse(int Id, int CustomerId, string CustomerName, string Status, decimal ShippingFee, DateTime Date, List<OrderItemResponse> Items);

public record OrderItemResponse(int Id, int ItemId, string ItemName, int Qty, decimal Price);

public record PickOrderResponse(int OrderId, string Status, string Message, List<PickedItemDetail> PickedItems);

public record PickedItemDetail(int ItemId, string ItemName, int Quantity, int BinId);

public record PackOrderResponse(int OrderId, string Status, string Message);

public record ShipOrderResponse(int OrderId, string Status, string Message);