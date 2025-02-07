using Convofy.Main.Models.User;

namespace Convofy.Main.Models.Forum;

public class ForumDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? FileLink { get; set; }
    public required UserSearchDto Owner { get; set; }
}