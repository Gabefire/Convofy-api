using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Convofy.Main.Interfaces;
using Convofy.Main.Models.Post;
using Convofy.Main.Models.UserVote;

namespace Convofy.Main.Controller;

[ApiController]
[Route("api/[controller]")]
public class VotingController(
    IValidator validate,
    IVotingService votingService) : ControllerBase
{
    private readonly IValidator _validate = validate;
    private readonly IVotingService _votingService = votingService;

    // GET all Posts by search
    [Authorize]
    [HttpPost("vote")]
    public async Task<IActionResult> Vote(UserVoteDto userVoteDto)
    {
        var user = await _validate.ValidateJwt(HttpContext);
        await _votingService.UpsertVote(user.Id, userVoteDto);
        return Ok();
    }

    // DELETE vote
    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteVote(Guid objectId)
    {
        var user = await _validate.ValidateJwt(HttpContext);
        var vote = await _votingService.GetVoteByUserIdAndObjectIdOrFail(user.Id, objectId);
        if (vote.CreatorUserId != user.Id)
        {
            return Forbid("Only the creator can delete this post");
        }
        await _votingService.DeleteVote(vote);
        return Ok();
    }

}