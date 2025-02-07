namespace Convofy.Main.Models.UserVote;

public class UserVoteDto
{
    public required Guid ObjectId { get; set; }
    public required ObjectType ObjectType { get; set; }
    public required bool IsUpVote { get; set; }
}

public enum ObjectType
{
    Post,
    Comment
}