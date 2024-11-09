namespace Convofy.Models;
public class UserSearchDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ProfilePicLink { get; set; } = string.Empty;
}