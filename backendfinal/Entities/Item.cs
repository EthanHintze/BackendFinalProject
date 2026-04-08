using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backendfinal.Entities;

[Table("item", Schema = "warehouse")]
public partial class Item
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("item_name")]
    [StringLength(50)]
    public string? ItemName { get; set; }

    [Column("price")]
    [Precision(10, 2)]
    public decimal? Price { get; set; }

    [Column("vendor_id")]
    public int? VendorId { get; set; }

    [Column("item_size")]
    [Precision(10, 2)]
    public decimal? ItemSize { get; set; }

    [InverseProperty("Item")]
    public virtual ICollection<BinLocation> BinLocations { get; set; } = new List<BinLocation>();

    [InverseProperty("Item")]
    public virtual ICollection<ItemShipment> ItemShipments { get; set; } = new List<ItemShipment>();

    [InverseProperty("Item")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [ForeignKey("VendorId")]
    [InverseProperty("Items")]
    public virtual Vendor? Vendor { get; set; }
}
