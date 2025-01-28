namespace Convofy.Models.Forum;

public class UserForumFollows
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    [ForeignKey("Forum")]
    public Guid ForumId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}