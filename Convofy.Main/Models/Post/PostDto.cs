namespace Convofy.Models.User;

public class PostDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UpVotes { get; set; }
    public int DownVotes { get; set; }
    public int Comments { get; set; }
    public ForumDto Forum { get; set; }
    public UserDto Owner { get; set; }
}