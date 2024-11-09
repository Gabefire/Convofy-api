using Convofy.Models;

namespace Convofy.Interfaces
{
    public interface IValidator
    {
        Task<User?> ValidateJwt(HttpContext context);
    }
}