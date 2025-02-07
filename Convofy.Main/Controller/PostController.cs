using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Convofy.Main.Interfaces;
using Convofy.Main.Models.Post;

namespace Convofy.Main.Controller;

[ApiController]
[Route("api/[controller]")]
public class PostController(
    IValidator validate,
    IPostService postService,
    IVotingService votingService) : ControllerBase
{
    private readonly IValidator _validate = validate;
    private readonly IPostService _postService = postService;
    private readonly IVotingService _votingService = votingService;
    // GET all Posts by search
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Posts([FromQuery] string search, [FromQuery] int limit = 10, [FromQuery] int offset = 0)
    {
        await _validate.ValidateJwt(HttpContext);

        if (limit <= 0 || offset < 0)
        {
            return BadRequest("Invalid pagination parameters. Limit must be positive and offset must be non-negative.");
        }

        var posts = await _postService.SearchPosts(search, limit, offset);

        return Ok(posts);
    }

    //POST create new forum
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreatePost(CreatePostDto request)
    {
        var user = await _validate.ValidateJwt(HttpContext);
        await _postService.CreatePost(request, user.Id);
        return Ok();
    }

    // PUT edit forum
    [Authorize]
    [HttpPut("edit")]
    public async Task<ActionResult> EditPost(EditPostDto editPostDto)
    {
        var user = await _validate.ValidateJwt(HttpContext);
        var post = await _postService.GetPostByIdOrFail(editPostDto.Id);
        if (post.CreatorUserId != user.Id)
        {
            return Forbid("Only the creator can edit this post");
        }

        await _postService.EditPost(post, editPostDto);
        return Ok();
    }

    // GET post by forum id
    [Authorize]
    [HttpGet("{forumId}")]
    public async Task<ActionResult> GetPostsByForumId(Guid forumId, [FromQuery] int limit = 10, [FromQuery] int offset = 0)
    {
        await _validate.ValidateJwt(HttpContext); ;

        var postList = await _postService.GetPostsByForumId(forumId, limit, offset);

        return Ok(postList);
    }

    // DELETE delete post
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePost(Guid id)
    {
        var user = await _validate.ValidateJwt(HttpContext);

        var post = await _postService.GetPostByIdOrFail(id);

        if (post.CreatorUserId != user.Id)
        {
            return Forbid("Only the creator can delete this post");
        }

        await _postService.DeletePost(post);
        return Ok();
    }

    // GET followed posts
    [Authorize]
    [HttpGet("followed")]
    public async Task<ActionResult> GetFollowedPosts([FromQuery] int limit = 10, [FromQuery] int offset = 0)
    {
        var user = await _validate.ValidateJwt(HttpContext);
        var posts = await _postService.GetFollowedPosts(user.Id, limit, offset);
        return Ok(posts);
    }

    [Authorize]
    [HttpGet("up-votes")]
    public async Task<ActionResult> GetUpVotes(Guid postId)
    {
        var upVotes = await _votingService.GetUpvoteCountByObjectId(postId);
        return Ok(upVotes);
    }

    [Authorize]
    [HttpGet("down-votes")]
    public async Task<ActionResult> GetDownVotes(Guid postId)
    {
        var downVotes = await _votingService.GetDownvoteCountByObjectId(postId);
        return Ok(downVotes);
    }
}