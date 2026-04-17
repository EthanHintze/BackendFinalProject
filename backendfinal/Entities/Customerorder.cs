using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backendfinal.Entities;

public class CustomerOrder
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal ShippingFee { get; set; }

    public int CustomerId { get; set; }

    public DateTime Date { get; set; }

    /// <summary>
    /// Valid values: CREATED, PICKED, PACKED, SHIPPED
    /// </summary>
    [MaxLength(20)]
    public string Status { get; set; } = "CREATED";

    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;

    public ICollection<CustomerOrderItem> OrderItems { get; set; } = [];
}