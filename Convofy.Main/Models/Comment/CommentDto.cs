using Convofy.Main.Models.User;

namespace Convofy.Main.Models.Comment;

public class CommentDto
{
    public required string Content { get; set; }
    public required UserSearchDto From { get; set; }
    public List<CommentDto> Children { get; set; } = [];
    public Guid PostId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Root { get; set; }
}

public class CreateCommentDto
{
    public required string Content { get; set; }
    public Guid? ParentCommentId { get; set; }
    public required Guid PostId { get; set; }
}

