using Convofy.Main.Models.Post;
using Convofy.Main.Models.UserVote;

namespace Convofy.Main.Interfaces;

public interface IVotingService
{
    Task<Post> GetObjectByIdOrFail(Guid id);
    Task<UserVote> GetVoteByUserIdAndObjectIdOrFail(Guid userId, Guid objectId);
    Task<UserVote> CreateVote(Guid userId, UserVoteDto userVoteDto);
    Task<UserVote> UpsertVote(Guid userId, UserVoteDto userVoteDto);
    Task DeleteVote(UserVote userVote);
    Task<int> GetUpvoteCountByObjectId(Guid objectId);
    Task<int> GetDownvoteCountByObjectId(Guid objectId);
}