using Convofy.Main.Models.User;

namespace Convofy.Main.Interfaces;
public interface IUserService
{
    Task<User?> GetById(Guid id);
}
