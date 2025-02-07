using Convofy.Main.Models.Comment;
using Convofy.Main.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Convofy.Main.Services;


public class CommentService(DatabaseContext context, ILogger<CommentService> logger) : ICommentService
{
    private readonly DatabaseContext _context = context;
    private readonly ILogger<CommentService> _logger = logger;

    public async Task<Comment> GetByIdOrFail(Guid id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            _logger.LogError("[CommentService] Comment not found: {Id}", id);
            throw new Exception("Comment not found");
        }
        return comment;
    }

    public async Task<Comment> CreateComment(CreateCommentDto createCommentDto, Guid userId)
    {
        var comment = new Comment
        {
            Content = createCommentDto.Content,
            CreatorUserId = userId,
            PostId = createCommentDto.PostId,
            ParentCommentId = createCommentDto.ParentCommentId,
        };
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task DeleteComment(Guid id)
    {
        var comment = await GetByIdOrFail(id);
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        _logger.LogInformation("[CommentService] Comment deleted: {Id}", id);
    }

    public async Task<List<Comment>> GetComments(int limit, int offset)
    {
        return await _context.Comments.Skip(offset).Take(limit).ToListAsync();
    }

    public async Task<List<Comment>> GetRootCommentsByPostId(Guid postId, int limit, int offset)
    {
        return await _context.Comments.Where(c => c.PostId == postId && c.ParentCommentId == null).Skip(offset).Take(limit).ToListAsync();
    }

    public async Task<List<Comment>> GetChildCommentsByCommentId(Guid commentId)
    {
        return await _context.Comments.Where(c => c.ParentCommentId == commentId).ToListAsync();
    }

    public async Task<int> GetCommentCountByPostId(Guid postId)
    {
        return await _context.Comments.CountAsync(c => c.PostId == postId);
    }
}