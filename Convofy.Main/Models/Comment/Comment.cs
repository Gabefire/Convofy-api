using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Convofy.Main.Models.Comment;

public class Comment
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [ForeignKey("User")]
    public required Guid CreatorUserId { get; set; }
    [ForeignKey("Post")]
    public required Guid PostId { get; set; }
    [ForeignKey("Comment")]
    public Guid? ParentCommentId { get; set; }
    [Required, MinLength(1)]
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}