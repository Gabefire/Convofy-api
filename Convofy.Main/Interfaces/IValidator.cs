using Convofy.Models.User;

namespace Convofy.Interfaces
{
    public interface IValidator
    {
        Task<User?> ValidateJwt(HttpContext context);
    }
}