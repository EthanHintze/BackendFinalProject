namespace backendfinal.Dtos;

// ═══════════════════════════════════════════════════════════════════
// INVENTORY
// ═══════════════════════════════════════════════════════════════════

public class InventoryReportResponse
{
    /// <summary>All inventory lines, optionally filtered by productId.</summary>
    public List<InventoryLineDto> Lines { get; set; } = [];

    /// <summary>Sum of Quantity across all returned lines.</summary>
    public int TotalQuantity { get; set; }
}

public class InventoryLineDto
{
    public int BinLocationId { get; set; }
    public int BinId { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = null!;
    public int Quantity { get; set; }

    // Where in the warehouse this bin lives
    public string? ShelfInfo { get; set; }
}

// ═══════════════════════════════════════════════════════════════════
// ORDER STATUS
// ═══════════════════════════════════════════════════════════════════

public class OrderStatusResponse
{
    public int OrderId { get; set; }
    public string Status { get; set; } = null!;
    public decimal ShippingFee { get; set; }
    public DateTime Date { get; set; }

    // Customer
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;

    // Line items
    public List<OrderStatusItemDto> Items { get; set; } = [];

    // Shipping info — only populated when Status == "SHIPPED"
    public ShippingInfoDto? ShippingInfo { get; set; }
}

public class OrderStatusItemDto
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = null!;
    public int Qty { get; set; }
    public decimal Price { get; set; }
}

/// <summary>Populated only when the order has been shipped.</summary>
public class ShippingInfoDto
{
    public DateTime ShippedAt { get; set; }
}