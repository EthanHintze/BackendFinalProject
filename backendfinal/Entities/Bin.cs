using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backendfinal.Entities;

[Table("bin", Schema = "warehouse")]
public partial class Bin
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("bin_type")]
    public int? BinType { get; set; }

    [InverseProperty("Bin")]
    public virtual ICollection<BinLocation> BinLocations { get; set; } = new List<BinLocation>();

    [ForeignKey("BinType")]
    [InverseProperty("Bins")]
    public virtual BinType? BinTypeNavigation { get; set; }
}
