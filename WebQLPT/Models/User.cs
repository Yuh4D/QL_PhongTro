using System.ComponentModel.DataAnnotations;
using WebQLPT.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    public string Role { get; set; }

    public bool IsApproved { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int? ChuTroId { get; set; }
    public ChuTro? ChuTro { get; set; }

    public int? KhachThueId { get; set; }
    public KhachThue? KhachThue { get; set; }
}