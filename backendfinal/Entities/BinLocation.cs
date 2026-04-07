using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backendfinal.Entities;

[Table("bin_location", Schema = "warehouse")]
public partial class BinLocation
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("shelf_id")]
    public int? ShelfId { get; set; }

    [Column("bin_id")]
    public int? BinId { get; set; }

    [Column("item_id")]
    public int? ItemId { get; set; }

    [Column("quantity")]
    public int? Quantity { get; set; }

    [ForeignKey("BinId")]
    [InverseProperty("BinLocations")]
    public virtual Bin? Bin { get; set; }

    [ForeignKey("ItemId")]
    [InverseProperty("BinLocations")]
    public virtual Item? Item { get; set; }

    [ForeignKey("ShelfId")]
    [InverseProperty("BinLocations")]
    public virtual Shelf? Shelf { get; set; }
}
