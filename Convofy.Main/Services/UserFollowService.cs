using Microsoft.EntityFrameworkCore;
using Convofy.Main.Models.Forum;
using Convofy.Main.Interfaces;

namespace Convofy.Main.Services
{
    public class UserFollowService(DatabaseContext context, ILogger<UserFollowService> logger) : IUserFollowService
    {
        private readonly DatabaseContext _context = context;
        private readonly ILogger<UserFollowService> _logger = logger;

        public async Task<UserForumFollows> GetUserForumFollowByIdOrFail(Guid id)
        {
            return await _context.UserForumFollows.FirstOrDefaultAsync(u => u.Id == id) ?? throw new Exception("UserForumFollow not found");
        }

        public async Task<UserForumFollows?> GetUserForumFollowByUserIdAndForumId(Guid userId, Guid forumId)
        {
            return await _context.UserForumFollows.FirstOrDefaultAsync(u => u.UserId == userId && u.ForumId == forumId);
        }

        public async Task<List<UserForumFollows>> GetUserForumFollowsByUserId(Guid userId, int limit, int offset)
        {
            return await _context.UserForumFollows.Where(u => u.UserId == userId).Skip(offset).Take(limit).ToListAsync();
        }

        public async Task<UserForumFollows> CreateUserForumFollow(UserForumFollows userForumFollows)
        {
            var userForumFollow = new UserForumFollows
            {
                UserId = userForumFollows.UserId,
                ForumId = userForumFollows.ForumId,
            };

            _context.UserForumFollows.Add(userForumFollow);
            await _context.SaveChangesAsync();
            _logger.LogInformation("[UserFollowService] UserForumFollow created successfully: {UserForumFollowId}", userForumFollow.Id);
            return userForumFollow;
        }
        public async Task DeleteUserForumFollow(Guid id)
        {
            var userForumFollow = await GetUserForumFollowByIdOrFail(id);

            _context.UserForumFollows.Remove(userForumFollow);
            await _context.SaveChangesAsync();
            _logger.LogInformation("[UserFollowService] UserForumFollow deleted successfully: {UserForumFollowId}", userForumFollow.Id);
        }
    }
}