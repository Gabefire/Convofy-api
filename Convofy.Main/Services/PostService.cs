using Microsoft.EntityFrameworkCore;
using Convofy.Main.Models.Post;
using Convofy.Main.Interfaces;

namespace Convofy.Main.Services;

public class PostService : IPostService
{
    private readonly DatabaseContext _context;
    private readonly ILogger<PostService> _logger;

    public PostService(DatabaseContext context, ILogger<PostService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Post> GetPostByIdOrFail(Guid id)
    {
        return await _context.Posts.FirstOrDefaultAsync(p => p.Id == id) ?? throw new Exception("Post not found");
    }

    public async Task<Post> CreatePost(CreatePostDto createPostDto, Guid userId)
    {
        var post = new Post
        {
            ForumId = createPostDto.ForumId,
            Content = createPostDto.Content,
            CreatorUserId = userId,
            Title = createPostDto.Title
        };
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        _logger.LogInformation("[PostService] Post created successfully {PostId}", post.Id);
        return post;
    }

    public async Task<Post> EditPost(Post post, EditPostDto editPostDto)
    {
        post.Content = editPostDto.Content ?? post.Content;
        post.Title = editPostDto.Title ?? post.Title;
        await _context.SaveChangesAsync();
        _logger.LogInformation("[PostService] Post edited successfully {PostId}", post.Id);
        return post;
    }

    public async Task DeletePost(Post post)
    {
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        _logger.LogInformation("[PostService] Post deleted successfully {PostId}", post.Id);
    }

    public async Task<List<Post>> GetPostsByForumId(Guid forumId, int limit, int offset)
    {
        return await _context.Posts
            .Where(p => p.ForumId == forumId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<Post>> SearchPosts(string search, int limit, int offset)
    {
        return await _context.Posts
            .Where(p => p.Title.Contains(search))
            .OrderByDescending(p => p.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<Post>> GetFollowedPosts(Guid userId, int limit, int offset)
    {
        return await _context.UserForumFollows
            .Where(uf => uf.UserId == userId)
            .Select(uf => uf.ForumId)
            .Join(_context.Posts, forumId => forumId, p => p.ForumId, (forumId, p) => p)
            .OrderByDescending(p => p.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }
}