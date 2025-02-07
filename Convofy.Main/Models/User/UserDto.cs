namespace Convofy.Main.Models.User;

public class UserDto
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public string? FileLink { get; set; }
}

public class UserLoginDto
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
}

public class UserSearchDto
{
    public required Guid Id { get; set; }
    public required string UserName { get; set; }
    public string? FileLink { get; set; }
}