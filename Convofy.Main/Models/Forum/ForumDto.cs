namespace Convofy.Main.Models.Forum;

public class ForumDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Content { get; set; }
    public string? Color { get; set; }
    public string? FileLink { get; set; }
}