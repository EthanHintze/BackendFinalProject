using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backendfinal.Entities;

[Table("shipment", Schema = "warehouse")]
public partial class Shipment
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("order_id")]
    public int? OrderId { get; set; }

    [Column("date_received", TypeName = "timestamp without time zone")]
    public DateTime? DateReceived { get; set; }

    [Column("price_adjust")]
    [Precision(10, 2)]
    public decimal? PriceAdjust { get; set; }

    [Column("handling_cost")]
    [Precision(10, 2)]
    public decimal? HandlingCost { get; set; }

    [InverseProperty("Shipment")]
    public virtual ICollection<ItemShipment> ItemShipments { get; set; } = new List<ItemShipment>();

    [ForeignKey("OrderId")]
    [InverseProperty("Shipments")]
    public virtual PurchaseOrder? Order { get; set; }
}
