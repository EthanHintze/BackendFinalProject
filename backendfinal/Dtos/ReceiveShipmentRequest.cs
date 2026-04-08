using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace backendfinal.Dtos;

public sealed class ReceiveShipmentRequest : IValidatableObject
{
    [Required(ErrorMessage = "ShipmentId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "ShipmentId must be greater than 0.")]
    public int ShipmentId { get; set; }

    [Required(ErrorMessage = "Shipment must include at least one item.")]
    [MinLength(1, ErrorMessage = "Shipment must include at least one item.")]
    public ICollection<ReceiveShipmentItemRequest> Items { get; set; } = new List<ReceiveShipmentItemRequest>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var item in Items.Select((value, index) => (value, index)))
        {
            if (item.value.Quantity <= 0)
            {
                yield return new ValidationResult("Quantity must be greater than 0.", new[] { $"Items[{item.index}].Quantity" });
            }

            if (item.value.ProductId <= 0)
            {
                yield return new ValidationResult("ProductId must be greater than 0.", new[] { $"Items[{item.index}].ProductId" });
            }
        }
    }
}
