using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Convofy.Models;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(100), Required]
    public string UserName { get; set; } = string.Empty;
    [MinLength(6), Required]
    public string? HashedPassword { get; set; }
    [EmailAddress, Required]
    public string Email { get; set; } = string.Empty;
    public string ProfilePicLink { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}