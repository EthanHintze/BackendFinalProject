using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace backendfinal.Dtos;

public sealed class CreatePurchaseOrderRequest : IValidatableObject
{
    [Required(ErrorMessage = "DateOrdered is required.")]
    public DateTime? DateOrdered { get; set; }

    [Required(ErrorMessage = "Purchase order must include at least one item.")]
    [MinLength(1, ErrorMessage = "Purchase order must include at least one item.")]
    public ICollection<CreatePurchaseOrderItemRequest> Items { get; set; } = new List<CreatePurchaseOrderItemRequest>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DateOrdered is not null && DateOrdered > DateTime.UtcNow)
        {
            yield return new ValidationResult("DateOrdered cannot be in the future.", new[] { nameof(DateOrdered) });
        }
    }
}
