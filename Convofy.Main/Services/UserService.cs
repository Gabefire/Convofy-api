using Convofy.Main.Models.User;
using Convofy.Main.Interfaces;

namespace Convofy.Main.Services;


public class UserService(DatabaseContext context, ILogger<UserService> logger) : IUserService
{
    private readonly DatabaseContext _context = context;
    private readonly ILogger<UserService> _logger = logger;

    public async Task<User?> GetById(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        return user;
    }
}