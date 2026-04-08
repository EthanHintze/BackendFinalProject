using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backendfinal.Entities;

[Table("purchase_order", Schema = "warehouse")]
public partial class PurchaseOrder
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("date_ordered", TypeName = "timestamp without time zone")]
    public DateTime? DateOrdered { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string? Status { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Order")]
    public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}
