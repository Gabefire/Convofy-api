using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Convofy.Models.Forum;
using Convofy.Models.Post;

namespace Convofy.Main.Services.ForumService;

public class ForumService
{
    private readonly DatabaseContext _context;
    private readonly ILogger<ForumService> _logger;

    public ForumService(DatabaseContext context, ILogger<ForumService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Forum> CreateForum(ForumDto forumDto)
    {
        var forum = new Forum
        {
            Title = forumDto.Title,
            Content = forumDto.Content,
            Color = forumDto.Color,
            FileLink = forumDto.FileLink
        };

        _context.Forums.Add(forum);
        await _context.SaveChangesAsync();
        _logger.LogInformation("[ForumService] Forum created successfully", forum.Id);
        return forum;
    }

    public async Task<Forum> EditForum(ForumDto forumDto, Guid userId)
    {
        var forum = await GetForumById(forumDto.Id);
        if (forum.CreatorUserId != userId)
        {
            throw new Exception("You are not the creator of this forum");
        }

        forum.Title = forumDto.Title ?? forum.Title;
        forum.Content = forumDto.Content ?? forum.Content;
        forum.Color = forumDto.Color ?? forum.Color;
        forum.FileLink = forumDto.FileLink ?? forum.FileLink;

        await _context.SaveChangesAsync();
        return forum;
    }

    public async Task<Forum> GetForumById(Guid id)
    {
        return await _context.Forums.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<List<Forum>> GetForums(int limit, int offset)
    {
        return await _context.Forums
            .OrderByDescending(f => f.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }
    public async Task<Void> DeleteForum(Guid id, Guid userId)
    {
        var forum = await GetForumById(id);
        if (forum.CreatorUserId != userId)
        {
            throw new Exception("You are not the creator of this forum");
        }
        _context.Forums.Remove(forum);
        await _context.SaveChangesAsync();
        _logger.LogInformation("[ForumService] Forum deleted successfully", forum.Id);
    }

    public async Task<List<Forum>> SearchForums(string search, int limit, int offset)
    {
        return await _context.Forums
            .Where(f => f.Title.Contains(search))
            .OrderByDescending(f => f.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<Forum>> GetFollowedForums(Guid userId, int limit, int offset)
    {
        return await _context.UserForumFollows
            .Where(uf => uf.UserId == userId)
            .Select(uf => uf.Forum)
            .OrderByDescending(f => f.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }
}