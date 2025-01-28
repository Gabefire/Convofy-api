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

namespace Convofy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForumController(IConfiguration configuration, DatabaseContext context, ILogger<UserController> logger, IValidator validate) : ControllerBase
{
    private readonly DatabaseContext _context = context;
    private readonly IConfiguration _configuration = configuration;
    private readonly IValidator _validate = validate;
    private readonly ILogger _logger = logger;

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

        var forums = await _context.Forums.Skip(offset).Take(limit).ToListAsync();

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

        var forum = new Forum { CreatorUserId = user.Id, Title = request.Title, Content = request.Content, Color = request.Color, FileLink = request.FileLink };
        _context.Forums.Add(forum);
        await _context.SaveChangesAsync();
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

        var existingForum = await _context.Forums.FirstOrDefaultAsync(f => f.Id == forumDto.Id);
        if (existingForum == null)
        {
            return NotFound("Forum not found");
        }

        if (existingForum.CreatorUserId != user.Id)
        {
            return Forbid("Only the creator can edit this forum");
        }

        existingForum.Title = forumDto.Title ?? existingForum.Title;
        existingForum.Content = forumDto.Content ?? existingForum.Content;
        existingForum.Color = forumDto.Color ?? existingForum.Color;

        await _context.SaveChangesAsync();
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

        var forumList = await _context.Forums
            .Where(x => x.Title.ToLower().Contains(name.ToLower()))
            .Select(x => new ForumSearchDto 
            { 
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                Color = x.Color,
                FileLink = x.FileLink 
            })
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
            
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

        _context.Forums.Remove(forum);
        await _context.SaveChangesAsync();
        _logger.LogInformation("[ForumController] Forum deleted successfully", id);
        return Ok();
    }
}