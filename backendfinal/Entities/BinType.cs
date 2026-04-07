using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backendfinal.Entities;

[Table("bin_type", Schema = "warehouse")]
public partial class BinType
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("height")]
    public int? Height { get; set; }

    [Column("length")]
    public int? Length { get; set; }

    [Column("width")]
    public int? Width { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string? Name { get; set; }

    [InverseProperty("BinTypeNavigation")]
    public virtual ICollection<Bin> Bins { get; set; } = new List<Bin>();
}
