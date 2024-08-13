using api_CodeFlow.Model.ModelsForAdd;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api_CodeFlow.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> Get(UserLogin userLogin)
        {
            var user = await Authentecate(userLogin);
            /*User user = ApplicationContext.Context.Users.FirstOrDefault(x => x.UserPassword == password && x.UserLogin == login);*/
            if (user == null)
            {
                return new ObjectResult("Неверный логин или пароль") { StatusCode = StatusCodes.Status401Unauthorized };
            }
            else
            {
                var token = Generate(user);
                return Ok(token);
            }
        }

        private string Generate(Model.User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthOptions.KEY));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserLogin),
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim(ClaimTypes.Role, user.UserRoleNavigation.RoleName),
            };

            var token = new JwtSecurityToken(AuthOptions.ISSUER, AuthOptions.AUDIENCE,
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<Model.User> Authentecate(UserLogin userLogin)
        {
            // Получаем пользователя по логину
            Model.User user = dbContext.Context.Users
                .Include(u => u.UserRoleNavigation)
                .FirstOrDefault(x => x.UserLogin == userLogin.Login);

            if (user == null)
            {
                return null;
            }
            else
            {
                // Проверяем пароль уже в памяти
                if (UserService.VerifyPassword(userLogin.Password, user.UserPassword))
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
