using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Convofy.Main.Models.UserVote;

[Index(nameof(ObjectId), nameof(CreatorUserId), IsUnique = true)]
public class UserVote
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [ForeignKey("User")]
    public required Guid CreatorUserId { get; set; }
    [ForeignKey("Post")]
    public required Guid ObjectId { get; set; }
    public required ObjectType ObjectType { get; set; }
    public required bool IsUpVote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}