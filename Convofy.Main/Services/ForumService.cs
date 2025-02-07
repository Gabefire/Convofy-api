using Microsoft.EntityFrameworkCore;
using Convofy.Main.Models.Forum;
using Convofy.Main.Interfaces;

namespace Convofy.Main.Services
{
    public class ForumService(DatabaseContext context, ILogger<ForumService> logger) : IForumService
    {
        private readonly DatabaseContext _context = context;
        private readonly ILogger<ForumService> _logger = logger;

        public async Task<Forum> CreateForum(ForumDto forumDto, Guid userId)
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

        public async Task<Forum> EditForum(Forum forum, ForumDto forumDto)
        {
            forum.Title = forumDto.Title ?? forum.Title;
            forum.Content = forumDto.Content ?? forum.Content;
            forum.Color = forumDto.Color ?? forum.Color;
            forum.FileLink = forumDto.FileLink ?? forum.FileLink;

            await _context.SaveChangesAsync();
            return forum;
        }

        public async Task<Forum> GetForumByIdOrFail(Guid id)
        {
            return await _context.Forums.FirstOrDefaultAsync(f => f.Id == id) ?? throw new Exception("Forum not found");
        }

        public async Task<List<Forum>> GetForums(int limit, int offset)
        {
            return await _context.Forums
                .OrderByDescending(f => f.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }
        public async Task DeleteForum(Guid id)
        {
            var forum = await GetForumByIdOrFail(id);

            _context.Forums.Remove(forum);
            await _context.SaveChangesAsync();
            _logger.LogInformation("[ForumService] Forum deleted successfully: {ForumId}", forum.Id);
        }

        public async Task<Forum> EditForum(ForumDto forumDto)
        {
            var forum = await GetForumByIdOrFail(forumDto.Id);

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