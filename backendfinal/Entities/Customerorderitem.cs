using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backendfinal.Entities;

public class CustomerOrderItem
{
    [Key]
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ItemId { get; set; }

    public int Qty { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal Price { get; set; }

    [ForeignKey(nameof(OrderId))]
    public CustomerOrder Order { get; set; } = null!;

    [ForeignKey(nameof(ItemId))]
    public Item Item { get; set; } = null!;
}