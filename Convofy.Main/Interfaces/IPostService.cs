using Convofy.Main.Models.Post;

namespace Convofy.Main.Interfaces;

public interface IPostService
{
    Task<Post> CreatePost(CreatePostDto createPostDto, Guid userId);
    Task<Post> EditPost(Post post, EditPostDto editPostDto);
    Task DeletePost(Post post);
    Task<Post> GetPostByIdOrFail(Guid id);
    Task<List<Post>> GetPostsByForumId(Guid forumId, int limit, int offset);
    Task<List<Post>> SearchPosts(string search, int limit, int offset);
    Task<List<Post>> GetFollowedPosts(Guid userId, int limit, int offset);
}
