using Microsoft.EntityFrameworkCore;
using Convofy.Main.Models.Post;
using Convofy.Main.Models.UserVote;
using Convofy.Main.Interfaces;

namespace Convofy.Main.Services;

public class VotingService : IVotingService
{
    private readonly DatabaseContext _context;
    private readonly ILogger<VotingService> _logger;

    public VotingService(DatabaseContext context, ILogger<VotingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Post> GetObjectByIdOrFail(Guid id)
    {
        return await _context.Posts.FirstOrDefaultAsync(v => v.Id == id) ?? throw new Exception("Vote not found");
    }

    public async Task<UserVote> GetVoteByUserIdAndObjectIdOrFail(Guid userId, Guid objectId)
    {
        return await _context.UserVotes.FirstOrDefaultAsync(v => v.CreatorUserId == userId && v.ObjectId == objectId) ?? throw new Exception("Vote not found");
    }

    public async Task<UserVote> CreateVote(Guid userId, UserVoteDto userVoteDto)
    {
        var userVote = new UserVote
        {
            CreatorUserId = userId,
            ObjectId = userVoteDto.ObjectId,
            ObjectType = userVoteDto.ObjectType,
            IsUpVote = userVoteDto.IsUpVote
        };
        _context.UserVotes.Add(userVote);
        await _context.SaveChangesAsync();
        _logger.LogInformation("[VotingService] Vote created successfully {VoteId}", userVote.Id);
        return userVote;
    }

    public async Task<UserVote> UpsertVote(Guid userId, UserVoteDto userVoteDto)
    {
        var existingVote = await _context.UserVotes
            .FirstOrDefaultAsync(v =>
                v.CreatorUserId == userId &&
                v.ObjectId == userVoteDto.ObjectId &&
                v.ObjectType == userVoteDto.ObjectType);

        if (existingVote != null)
        {
            existingVote.IsUpVote = userVoteDto.IsUpVote;
            _context.UserVotes.Update(existingVote);
            _logger.LogInformation("[VotingService] Vote updated successfully {VoteId}", existingVote.Id);
        }
        else
        {
            existingVote = new UserVote
            {
                CreatorUserId = userId,
                ObjectId = userVoteDto.ObjectId,
                ObjectType = userVoteDto.ObjectType,
                IsUpVote = userVoteDto.IsUpVote
            };
            _context.UserVotes.Add(existingVote);
            _logger.LogInformation("[VotingService] Vote created successfully {VoteId}", existingVote.Id);
        }

        await _context.SaveChangesAsync();
        return existingVote;
    }

    public async Task DeleteVote(UserVote userVote)
    {
        _context.UserVotes.Remove(userVote);
        await _context.SaveChangesAsync();
        _logger.LogInformation("[VotingService] Vote deleted successfully {VoteId}", userVote.Id);
    }

    public async Task<int> GetUpvoteCountByObjectId(Guid objectId)
    {
        return await _context.UserVotes
            .Where(v => v.ObjectId == objectId && v.IsUpVote)
            .CountAsync();
    }

    public async Task<int> GetDownvoteCountByObjectId(Guid objectId)
    {
        return await _context.UserVotes
            .Where(v => v.ObjectId == objectId && !v.IsUpVote)
            .CountAsync();
    }

    public async Task<(int upvotes, int downvotes)> GetUpvoteAndDownvoteCountByObjectId(Guid objectId)
    {
        var votes = await _context.UserVotes
            .Where(v => v.ObjectId == objectId)
            .GroupBy(v => v.IsUpVote)
            .Select(g => new { IsUpVote = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.IsUpVote, x => x.Count);

        return (
            upvotes: votes.GetValueOrDefault(true, 0),
            downvotes: votes.GetValueOrDefault(false, 0)
        );
    }
}
