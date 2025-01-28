using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Convofy.Services;
using Microsoft.EntityFrameworkCore;
using Convofy.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Convofy.Models.User;
using Convofy.Main.Services.ForumService;

namespace Convofy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForumController(IConfiguration configuration, DatabaseContext context, ILogger<UserController> logger, IValidator validate) : ControllerBase
{
    private readonly DatabaseContext _context = context;
    private readonly IConfiguration _configuration = configuration;
    private readonly IValidator _validate = validate;
    private readonly ILogger _logger = logger;
    private readonly ForumService _forumService = forumService;
    // GET all Forums
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Forums([FromQuery] int limit = 10, [FromQuery] int offset = 0)
    {
        var user = await _validate.ValidateJwt(HttpContext);
        if (user == null)
        {
            return Unauthorized();
        }

        if (limit <= 0 || offset < 0)
        {
            return BadRequest("Invalid pagination parameters. Limit must be positive and offset must be non-negative.");
        }

        var forums = await _forumService.GetForums(limit, offset);

        return Ok(forums);
    }

    //POST create new forum
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateForum(ForumDto request)
    {
        var user = await _validate.ValidateJwt(HttpContext);
        if (user == null)
        {
            return Unauthorized();
        }

        await _forumService.CreateForum(request);
        return Ok();
    }

    // PUT edit forum
    [Authorize]
    [HttpPut("edit")]
    public async Task<ActionResult> EditForum(ForumDto forumDto)
    {
        var user = await _validate.ValidateJwt(HttpContext);
        if (user == null)
        {
            return Unauthorized();
        }

        await _forumService.EditForum(forumDto, user.Id);
        return Ok();
    }

    // GET search for forum
    [Authorize]
    [HttpGet("{name}")]
    public async Task<ActionResult> SearchForum(string name, [FromQuery] int limit = 10, [FromQuery] int offset = 0)
    {
        // Validate JWT and get user
        var user = await _validate.ValidateJwt(HttpContext);
        if (user == null)
        {
            return Unauthorized();
        }

        var forumList = await _forumService.SearchForums(name, limit, offset);

        return Ok(forumList);
    }

    // DELETE delete forum
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteForum(Guid id)
    {
        var user = await _validate.ValidateJwt(HttpContext);
        if (user == null)
        {
            return Unauthorized();
        }

        var forum = await _context.Forums.FirstOrDefaultAsync(f => f.Id == id);
        if (forum == null)
        {
            return NotFound("Forum not found");
        }

        if (forum.CreatorUserId != user.Id)
        {
            return Forbid("Only the creator can delete this forum");
        }

        await _forumService.DeleteForum(id, user.Id);
        return Ok();
    }
}