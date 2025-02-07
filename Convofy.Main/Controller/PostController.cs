using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Convofy.Main.Interfaces;
using Convofy.Main.Models.Post;
using Convofy.Main.Models.Comment;
using Convofy.Main.Models.User;
namespace Convofy.Main.Controller;

[ApiController]
[Route("api/[controller]")]
public class PostController(
    IValidator validate,
    IPostService postService,
    IVotingService votingService,
    ICommentService commentService,
    IUserService userService,
    IUserFollowService userFollowService) : ControllerBase
{
    private readonly IValidator _validate = validate;
    private readonly IPostService _postService = postService;
    private readonly IVotingService _votingService = votingService;
    private readonly ICommentService _commentService = commentService;
    private readonly IUserService _userService = userService;
    private readonly IUserFollowService _userFollowService = userFollowService;
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

        var postDtos = new List<PostDto>();
        foreach (var post in postList)
        {
            var (upVoteCount, downVoteCount) = await _votingService.GetUpvoteAndDownvoteCountByObjectId(post.Id);
            var commentCount = await _commentService.GetCommentCountByPostId(post.Id);
            var user = await _userService.GetById(post.CreatorUserId);
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
                    Id = post.CreatorUserId,
                    UserName = "Deleted User",
                };
            }

            var postDto = new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                UpVotes = upVoteCount,
                DownVotes = downVoteCount,
                Comments = commentCount,
                Forum = null,
                Owner = owner,
            };
            postDtos.Add(postDto);
        }

        return Ok(postDtos);
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

    [Authorize]
    [HttpGet("comments")]
    public async Task<ActionResult> GetComments(Guid postId, [FromQuery] int limit = 10, [FromQuery] int offset = 0)
    {
        var comments = await _commentService.GetRootCommentsByPostId(postId, limit, offset);
        var commentDtos = new List<CommentDto>();
        foreach (var comment in comments)
        {
            var user = await _userService.GetById(comment.CreatorUserId);
            UserSearchDto? userDto = null;
            if (user == null)
            {
                userDto = new UserSearchDto
                {
                    Id = comment.CreatorUserId,
                    UserName = "Deleted User",
                };
            }
            else
            {
                userDto = new UserSearchDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FileLink = user.FileLink,
                };
            }
            var commentDto = new CommentDto
            {
                Content = comment.Content,
                From = userDto,
                Children = [],
                UpdatedAt = comment.UpdatedAt,
                PostId = postId,
            };
            var childDtos = await GetAllCommentChildrenDtos(comment, commentDto.Children, postId);
            commentDto.Children = childDtos;
            commentDtos.Add(commentDto);
        }
        return Ok(commentDtos);
    }

    private async Task<List<CommentDto>> GetAllCommentChildrenDtos(Comment comment, List<CommentDto> commentDtos, Guid postId)
    {
        var children = await _commentService.GetChildCommentsByCommentId(comment.Id);
        foreach (var child in children)
        {
            var user = await _userService.GetById(child.CreatorUserId);
            UserSearchDto? userDto = null;
            if (user == null)
            {
                userDto = new UserSearchDto
                {
                    Id = child.CreatorUserId,
                    UserName = "Deleted User",
                };
            }
            else
            {
                userDto = new UserSearchDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FileLink = user.FileLink,
                };
            }
            var commentDto = new CommentDto
            {
                Content = child.Content,
                From = userDto,
                Children = [],
                UpdatedAt = child.UpdatedAt,
                PostId = postId,
            };
            var childChildren = await GetAllCommentChildrenDtos(child, commentDtos, postId);
            commentDto.Children = childChildren;
            commentDtos.Add(commentDto);
        }
        return commentDtos;
    }
}