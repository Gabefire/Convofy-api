using System.IdentityModel.Tokens.Jwt;
using Convofy.Main.Models.User;
using Convofy.Main.Services;
using Convofy.Main.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Convofy.Main.Util
{
    public class Validator(DatabaseContext context) : IValidator
    {
        private readonly DatabaseContext _context = context;
        public async Task<User> ValidateJwt(HttpContext context)
        {
            //JWT for user ID
            string token = context.Request.Headers.Authorization.ToString();
            var handler = new JwtSecurityTokenHandler();

            //Check if JWT can be read
            if (token.IsNullOrEmpty() || token.Split(" ").Length < 2)
            {
                throw new UnauthorizedAccessException();
            }
            ;

            if (!handler.CanReadToken(token.Split(" ")[1]))
            {
                throw new UnauthorizedAccessException();
            }
            ;

            if (handler.ReadToken(token.Split(" ")[1]) is not JwtSecurityToken jwtToken)
            {
                throw new UnauthorizedAccessException();
            }

            string Id = jwtToken.Claims.First(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

            if (!int.TryParse(Id, out int userId))
            {
                throw new UnauthorizedAccessException();
            }

            //Validate and get user
            var user = await _context.Users.FindAsync(userId) ?? throw new UnauthorizedAccessException();
            return user;
        }
    }
}
