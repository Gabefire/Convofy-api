using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Convofy.Main.Models.Post;

public class Post
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [ForeignKey("User")]
    public required Guid CreatorUserId { get; set; }
    [ForeignKey("Forum")]
    public required Guid ForumId { get; set; }
    [MinLength(1), MaxLength(20)]
    public required string Title { get; set; }
    [Required, MinLength(1)]
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}