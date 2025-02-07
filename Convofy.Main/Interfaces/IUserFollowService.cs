using Convofy.Main.Models.Forum;

namespace Convofy.Main.Interfaces;
public interface IUserFollowService
{
    Task<UserForumFollows> GetUserForumFollowByIdOrFail(Guid id);
    Task<UserForumFollows> CreateUserForumFollow(UserForumFollows userForumFollows);
    Task DeleteUserForumFollow(Guid id);
    Task<List<UserForumFollows>> GetUserForumFollowsByUserId(Guid userId, int limit, int offset);
    Task<UserForumFollows?> GetUserForumFollowByUserIdAndForumId(Guid userId, Guid forumId);
}
