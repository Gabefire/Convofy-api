using Convofy.Main.Models.User;

namespace Convofy.Main.Interfaces
{
    public interface IValidator
    {
        Task<User> ValidateJwt(HttpContext context);
    }
}