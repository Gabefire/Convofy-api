using Convofy.Main.Models.Forum;
using Convofy.Main.Models.User;

namespace Convofy.Main.Models.Post;

public class PostDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UpVotes { get; set; }
    public int DownVotes { get; set; }
    public int Comments { get; set; }
    public required ForumDto Forum { get; set; }
    public required UserDto Owner { get; set; }
}

public class CreatePostDto
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required Guid ForumId { get; set; }
    public required Guid OwnerId { get; set; }
}

public class EditPostDto
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required Guid Id { get; set; }
}