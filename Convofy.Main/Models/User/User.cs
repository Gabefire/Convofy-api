using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Convofy.Models.User;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(100), Required]
    public required string UserName { get; set; }
    [MinLength(6), Required]
    public required string HashedPassword { get; set; }
    [EmailAddress, Required]
    public required string Email { get; set; }
    public string? ProfilePicLink { get; set; } = null;
    public string? Color { get; set; } = null;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}