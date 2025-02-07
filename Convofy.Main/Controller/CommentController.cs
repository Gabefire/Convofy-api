using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Convofy.Main.Interfaces;
using Convofy.Main.Models.Comment;

namespace Convofy.Main.Controller;

[ApiController]
[Route("api/[controller]")]
public class CommentController(
    IValidator validate,
    ICommentService commentService) : ControllerBase
{
    private readonly IValidator _validate = validate;
    private readonly ICommentService _commentService = commentService;
    // GET all Forums
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Comments([FromQuery] int limit = 10, [FromQuery] int offset = 0)
    {
        await _validate.ValidateJwt(HttpContext);

        if (limit <= 0 || offset < 0)
        {
            return BadRequest("Invalid pagination parameters. Limit must be positive and offset must be non-negative.");
        }

        var comments = await _commentService.GetComments(limit, offset);

        return Ok(comments);
    }

    //POST create new forum
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateComment(CreateCommentDto request)
    {
        var user = await _validate.ValidateJwt(HttpContext);
        await _commentService.CreateComment(request, user.Id);
        return Ok();
    }

    // DELETE delete comment
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteComment(Guid id)
    {
        var user = await _validate.ValidateJwt(HttpContext);

        var comment = await _commentService.GetByIdOrFail(id);

        if (comment.CreatorUserId != user.Id)
        {
            return Forbid("Only the creator can delete this comment");
        }

        await _commentService.DeleteComment(id);
        return Ok();
    }
}