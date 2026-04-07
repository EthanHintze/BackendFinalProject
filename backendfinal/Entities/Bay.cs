using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backendfinal.Entities;

[Table("bay", Schema = "warehouse")]
public partial class Bay
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("aisle_id")]
    public int? AisleId { get; set; }

    [ForeignKey("AisleId")]
    [InverseProperty("Bays")]
    public virtual Aisle? Aisle { get; set; }

    [InverseProperty("Bay")]
    public virtual ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();
}
