using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backendfinal.Entities;

[Table("item_shipment", Schema = "warehouse")]
public partial class ItemShipment
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("item_id")]
    public int? ItemId { get; set; }

    [Column("shipment_id")]
    public int ShipmentId { get; set; }

    [Column("action_id")]
    public int? ActionId { get; set; }

    [Column("discount")]
    [Precision(10, 2)]
    public decimal? Discount { get; set; }

    [Column("quantity")]
    public int? Quantity { get; set; }

    [ForeignKey("ActionId")]
    [InverseProperty("ItemShipments")]
    public virtual ShipmentAction? Action { get; set; }

    [ForeignKey("ItemId")]
    [InverseProperty("ItemShipments")]
    public virtual Item? Item { get; set; }

    [ForeignKey("ShipmentId")]
    [InverseProperty("ItemShipments")]
    public virtual Shipment Shipment { get; set; } = null!;
}
