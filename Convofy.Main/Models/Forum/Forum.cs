using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Convofy.Main.Models.Forum;

[Index(nameof(Title), IsUnique = true)]
public class Forum
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [ForeignKey("User")]
    public required Guid CreatorUserId { get; set; }
    [MinLength(1), MaxLength(20)]
    public required string Title { get; set; }
    [MinLength(1), MaxLength(600)]
    public string? Description { get; set; } = null;
    public required string Color { get; set; } = "#DC2626";
    public string? FileLink { get; set; } = null;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}