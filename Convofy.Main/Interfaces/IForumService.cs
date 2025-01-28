using Convofy.Main.Models.Forum;

namespace Convofy.Main.Interfaces;
public interface IForumService
{
    Task<Forum?> GetForumById(Guid id);
    Task<Forum?> CreateForum(ForumDto forumDto, Guid userId);
    Task DeleteForum(Guid id, Guid userId);
    Task<List<Forum>> GetForums(int limit, int offset);
    Task<List<Forum>> SearchForums(string search, int limit, int offset);
    Task<List<Forum>> GetFollowedForums(Guid userId, int limit, int offset);
    Task<Forum> EditForum(ForumDto forumDto);
}
