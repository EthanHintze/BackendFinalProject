using System.ComponentModel.DataAnnotations;

namespace backendfinal.Entities;

public class Customer
{
    [Key]
    public int Id { get; set; }

    [MaxLength(30)]
    public string? Name { get; set; }

    [MaxLength(50)]
    public string? Email { get; set; }

    public int? Phone { get; set; }

    [MaxLength(50)]
    public string? Address { get; set; }

    public ICollection<CustomerOrder> Orders { get; set; } = [];
}