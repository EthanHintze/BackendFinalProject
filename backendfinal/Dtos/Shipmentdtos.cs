using System.ComponentModel.DataAnnotations;

namespace backendfinal.Dtos;

// ── Request: Create Shipment ──────────────────────────────────────────────────

public class CreateShipmentRequest
{
    [Required]
    public int PurchaseOrderId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Handling cost cannot be negative.")]
    public decimal HandlingCost { get; set; }
}

// ── Response: Shipment ────────────────────────────────────────────────────────

public class ShipmentResponse
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public decimal HandlingCost { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? DateReceived { get; set; }
    public bool IsReceived { get; set; }
}

