using System.ComponentModel.DataAnnotations;

namespace backendfinal.Dtos;

public sealed class CreatePurchaseOrderRequest
{
    [Required(ErrorMessage = "DateOrdered is required.")]
    public DateTime DateOrdered { get; set; }
}
