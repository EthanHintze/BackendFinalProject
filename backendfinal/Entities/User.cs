using System.ComponentModel.DataAnnotations;

namespace backendfinal.Entities;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Username { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;
}