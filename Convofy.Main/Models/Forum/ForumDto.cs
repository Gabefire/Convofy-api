namespace Convofy.Models.User;

public class ForumDto
{
    public required string Title { get; set; }
    public string? Content { get; set; }
    public string Color { get; set; } = "#DC2626";
    public string? FileLink { get; set; }
}