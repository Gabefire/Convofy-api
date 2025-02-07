using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Convofy.Main.Interfaces;
using Convofy.Main.Models.Forum;
using Convofy.Main.Models.User;

namespace Convofy.Main.Controller;

[ApiController]
[Route("api/[controller]")]
public class ForumController(
    IValidator validate,
    IForumService forumService,
    IUserFollowService userFollowService,
    IPostService postService,
    IUserService userService) : ControllerBase
{
    private readonly IValidator _validate = validate;
    private readonly IForumService _forumService = forumService;
    private readonly IUserFollowService _userFollowService = userFollowService;
    private readonly IPostService _postService = postService;
    private readonly IUserService _userService = userService;
    // GET all Forums
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Forums([FromQuery] int limit = 10, [FromQuery] int offset = 0)
    {
        await _validate.ValidateJwt(HttpContext);

        if (limit <= 0 || offset < 0)
        {
            return BadRequest("Invalid pagination parameters. Limit must be positive and offset must be non-negative.");
        }

        var forums = await _forumService.GetForums(limit, offset);

        return Ok(forums);
    }

    // Get forum
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetForum(Guid id)
    {
        var forum = await _forumService.GetForumByIdOrFail(id);
        var user = await _userService.GetById(forum.CreatorUserId);

        UserSearchDto? owner = null;
        if (user != null)
        {
            owner = new UserSearchDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FileLink = user.FileLink,
            };
        }
        else
        {
            owner = new UserSearchDto
            {
                Id = forum.CreatorUserId,
                UserName = "Deleted User",
            };
        }
        var forumDto = new ForumDto
        {
            Id = forum.Id,
            Title = forum.Title,
            Description = forum.Description,
            Color = forum.Color,
            FileLink = forum.FileLink,
            Owner = owner,
        };
        return Ok(forumDto);
    }

    //POST create new forum
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateForum(ForumDto request)
    {
        var user = await _validate.ValidateJwt(HttpContext);
        var forum = await _forumService.GetForumByTitle(request.Title);
        if (forum != null)
        {
            return Conflict("Forum with this title already exists");
        }
        var createdForum = await _forumService.CreateForum(request, user.Id);

        var userForumFollow = new UserForumFollows
        {
            UserId = user.Id,
            ForumId = createdForum.Id,
        };
        await _userFollowService.CreateUserForumFollow(userForumFollow);

        var forumDto = new ForumDto
        {
            Id = createdForum.Id,
            Title = createdForum.Title,
            Description = createdForum.Description,
            Color = createdForum.Color,
            FileLink = createdForum.FileLink,
            Owner = new UserSearchDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FileLink = user.FileLink,
            },
        };
        return Ok(forumDto);
    }

    // POST follow forum
    [Authorize]
    [HttpPost("follow")]
    public async Task<IActionResult> FollowForum(Guid forumId)
    {
        var user = await _validate.ValidateJwt(HttpContext);
        var userForumFollow = await _userFollowService.GetUserForumFollowByUserIdAndForumId(user.Id, forumId);
        if (userForumFollow != null)
        {
            return Conflict("You are already following this forum");
        }
        var newUserForumFollow = new UserForumFollows
        {
            UserId = user.Id,
            ForumId = forumId,
        };
        await _userFollowService.CreateUserForumFollow(newUserForumFollow);
        return Ok();
    }

    // POST unfollow forum
    [Authorize]
    [HttpPost("unfollow")]
    public async Task<IActionResult> UnfollowForum(Guid forumId)
    {
        var user = await _validate.ValidateJwt(HttpContext);
        var userForumFollow = await _userFollowService.GetUserForumFollowByUserIdAndForumId(user.Id, forumId);
        if (userForumFollow == null)
        {
            return Conflict("You are not following this forum");
        }
        await _userFollowService.DeleteUserForumFollow(userForumFollow.Id);
        return Ok();
    }

    // PUT edit forum
    [Authorize]
    [HttpPut("edit")]
    public async Task<ActionResult> EditForum(ForumDto forumDto)
    {
        var user = await _validate.ValidateJwt(HttpContext);
        var forum = await _forumService.GetForumByIdOrFail(forumDto.Id);
        if (forum.CreatorUserId != user.Id)
        {
            return Forbid("Only the creator can edit this forum");
        }

        await _forumService.EditForum(forum, forumDto);
        return Ok();
    }

    // GET search for forum
    [Authorize]
    [HttpGet]
    public async Task<ActionResult> SearchForum([FromQuery] string search, [FromQuery] int limit = 10, [FromQuery] int offset = 0)
    {
        await _validate.ValidateJwt(HttpContext);

        var forumList = await _forumService.SearchForums(search, limit, offset);

        return Ok(forumList);
    }

    // DELETE delete forum
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteForum(Guid id)
    {
        var user = await _validate.ValidateJwt(HttpContext);

        var forum = await _forumService.GetForumByIdOrFail(id);

        if (forum.CreatorUserId != user.Id)
        {
            return Forbid("Only the creator can delete this forum");
        }

        var posts = await _postService.GetPostsByForumId(id, 1, 0);
        if (posts.Count > 0)
        {
            return BadRequest("This forum has posts and cannot be deleted");
        }

        await _forumService.DeleteForum(id);
        return Ok();
    }
}