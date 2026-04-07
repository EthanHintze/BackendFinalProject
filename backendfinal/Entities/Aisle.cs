using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backendfinal.Entities;

[Table("aisle", Schema = "warehouse")]
public partial class Aisle
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [InverseProperty("Aisle")]
    public virtual ICollection<Bay> Bays { get; set; } = new List<Bay>();
}
