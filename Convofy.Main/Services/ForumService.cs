using Microsoft.EntityFrameworkCore;
using Convofy.Main.Models.Forum;
using Convofy.Main.Interfaces;

namespace Convofy.Main.Services
{
    public class ForumService : IForumService
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<ForumService> _logger;

        public ForumService(DatabaseContext context, ILogger<ForumService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Forum?> CreateForum(ForumDto forumDto, Guid userId)
        {
            var forum = new Forum
            {
                Title = forumDto.Title,
                Content = forumDto.Content,
                Color = forumDto.Color ?? "#000000",
                FileLink = forumDto.FileLink,
                CreatorUserId = userId
            };

            _context.Forums.Add(forum);
            await _context.SaveChangesAsync();
            _logger.LogInformation("[ForumService] Forum created successfully: {ForumId}", forum.Id);
            return forum;
        }

        public async Task<Forum> EditForum(ForumDto forumDto, Guid userId)
        {
            var forum = await GetForumById(forumDto.Id);
            if (forum == null || forum.CreatorUserId != userId)
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

        public async Task<Forum?> GetForumById(Guid id)
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
        public async Task DeleteForum(Guid id, Guid userId)
        {
            var forum = await GetForumById(id);
            if (forum == null || forum.CreatorUserId != userId)
            {
                throw new Exception("You are not the creator of this forum");
            }
            _context.Forums.Remove(forum);
            await _context.SaveChangesAsync();
            _logger.LogInformation("[ForumService] Forum deleted successfully: {ForumId}", forum.Id);
        }

        public async Task<Forum> EditForum(ForumDto forumDto)
        {
            var forum = await GetForumById(forumDto.Id) ?? throw new Exception("Forum not found");

            forum.Title = forumDto.Title ?? forum.Title;
            forum.Content = forumDto.Content ?? forum.Content;
            forum.Color = forumDto.Color ?? forum.Color;
            forum.FileLink = forumDto.FileLink ?? forum.FileLink;
            await _context.SaveChangesAsync();
            _logger.LogInformation("[ForumService] Forum edited successfully: {ForumId}", forum.Id);
            return forum;
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
                .Select(uf => uf.ForumId)
                .Join(_context.Forums, id => id, f => f.Id, (id, f) => f)
                .OrderByDescending(f => f.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }
    }
}