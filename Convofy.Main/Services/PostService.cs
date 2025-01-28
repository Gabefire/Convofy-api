using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Convofy.Main.Services;
using Convofy.Main.Models.Post;

namespace Convofy.Main.Services.PostService;

public class PostService
{
    private readonly DatabaseContext _context;
    private readonly ILogger<PostService> _logger;

    public PostService(DatabaseContext context, ILogger<PostService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Post> CreatePost(PostDto postDto)
    {
        var post = new Post { ForumId = postDto.ForumId, Content = postDto.Content, FileLink = postDto.FileLink };
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        _logger.LogInformation("[PostService] Post created successfully", post.Id);
        return post;
    }

    public async Task<Post> EditPost(PostDto postDto, Guid userId)
    {
        var post = await GetPostById(postDto.Id);
        if (post.CreatorUserId != userId)
        {
            throw new Exception("You are not the creator of this post");
        }

        post.Content = postDto.Content ?? post.Content;
        post.FileLink = postDto.FileLink ?? post.FileLink;
        await _context.SaveChangesAsync();
        _logger.LogInformation("[PostService] Post edited successfully", post.Id);
        return post;
    }

    public async Task<Post> DeletePost(Guid id, Guid userId)
    {
        var post = await GetPostById(id);
        if (post.CreatorUserId != userId)
        {
            throw new Exception("You are not the creator of this post");
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        _logger.LogInformation("[PostService] Post deleted successfully", post.Id);
        return post;
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

    public async Task<List<Post>> GetUsers(string search, int limit, int offset)
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