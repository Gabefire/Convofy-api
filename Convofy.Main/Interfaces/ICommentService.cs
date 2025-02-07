using Convofy.Main.Models.Comment;

namespace Convofy.Main.Interfaces;
public interface ICommentService
{
    Task<Comment> GetByIdOrFail(Guid id);
    Task<Comment> CreateComment(CreateCommentDto createCommentDto, Guid userId);
    Task DeleteComment(Guid id);
    Task<List<Comment>> GetComments(int limit, int offset);
    Task<List<Comment>> GetRootCommentsByPostId(Guid postId, int limit, int offset);
    Task<List<Comment>> GetChildCommentsByCommentId(Guid commentId);
}
