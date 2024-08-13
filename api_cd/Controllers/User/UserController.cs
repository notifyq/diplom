using api_CodeFlow.Model;
using api_CodeFlow.Model.ModelsForAdd;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace api_CodeFlow.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("Library")]
        [Authorize]
        public async Task<ActionResult<List<Model.Product>>> GetLibrary()
        {
            var currentUser = GetCurrectUser();
            if (currentUser == null)
            {
                return BadRequest();
            }

            List<Model.Product> products = new List<Model.Product>();
            products = dbContext.Context.Products
                .Include(x => x.PurchaseLists)
                .ThenInclude(xl => xl.Purchase).ToList();
            products = products.Where(x => x.PurchaseLists.Any(p => p.Purchase.UserId == currentUser.UserId)).ToList();


            return Ok(products);
        }
        /*[HttpGet]
        [Authorize]
        [Route("Library/Keys")]
        public async Task<ActionResult<List<ProductKey>>> GetProductsKeys()
        {
            var currentUser = GetCurrectUser();
            if (currentUser == null)
            {
                return BadRequest();
            }

            List<ProductKey> keys = dbContext.Context.ProductKeys.Include(p => p.Product).Where(x => x.UserId == currentUser.UserId).ToList();

            return Ok(keys);
        }*/
        [HttpGet]
        [Route("Purchases")]
        [Authorize]
        public async Task<ActionResult<List<Purchase>>> GetPurchases()
        {
            var currentUser = GetCurrectUser();
            if (currentUser == null)
            {
                return BadRequest();
            }

            List<Purchase> purchases = dbContext.Context.Purchases
                .Include(x => x.PurchaseLists)
                .ThenInclude(x => x.Product)
                .Include(x => x.PurchaseStatusNavigation)
                .Where(x => x.UserId == currentUser.UserId)
                .ToList();
            if (purchases.Count == 0)
            {
                return BadRequest();
            }
            return Ok(purchases);

        }
        [HttpGet]
        [Route("Library/{user_id}")]
        public async Task<ActionResult<List<Model.Product>>> GetUserLibrary(int user_id)
        {
            if (user_id == null)
            {
                return BadRequest();
            }
            List<Model.Product> products = new List<Model.Product>();
            products = dbContext.Context.Products
                .Include(x => x.PurchaseLists)
                .ThenInclude(xl => xl.Purchase).ToList();

            products = products.Where(x => x.PurchaseLists.Any(p => p.Purchase.UserId == user_id)).ToList();
            return products;
        }

        [HttpGet]
        [Route("GetCurrentUserInfo")]
        public async Task<ActionResult<Model.User>> GetCurrentUserInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;

                var user = new Model.User
                {
                    UserId = Convert.ToInt32(userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value),
                    UserLogin = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,
                    UserEmail = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                    UserRoleNavigation = new Role { RoleName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value, }
                };

                var dbUser = await dbContext.Context.Users.FindAsync(user.UserId);
                if (dbUser != null)
                {
                    user.UserId = dbUser.UserId;
                    user.UserName = dbUser.UserName;
                    user.UserRoleNavigation = dbUser.UserRoleNavigation;
                    user.UserLogin = dbUser.UserLogin;
                    user.UserEmail = dbUser.UserEmail;
                }

                return Ok(user);
            }
            return null;
        }

        [HttpPut]
        [Authorize]
        [Route("EditUserInfo")]
        public async Task<IActionResult> EditUserInfo(Model.User user)
        {
            Model.User currentUser = GetCurrectUser();
            var dbUser = await dbContext.Context.Users.FindAsync(currentUser.UserId);
            if (dbUser == null)
            {
                return NotFound();
            }

            // Обновляем информацию о пользователе
            try
            {
                dbUser.UserName = user.UserName;
            /*    dbUser.UserEmail = user.UserEmail;
                dbUser.UserPassword = UserService.HashPassword(user.UserPassword);*/

                dbContext.Context.Entry(dbUser).State = EntityState.Modified;
                await dbContext.Context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(currentUser.UserId))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest("");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка: " + ex.Message);
            }
        }

        private bool UserExists(int id)
        {
            return dbContext.Context.Users.Any(e => e.UserId == id);
        }
        private Model.User GetCurrectUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new Model.User
                {
                    UserId = Convert.ToInt32(userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value),
                    UserLogin = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,
                    UserEmail = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                    UserRoleNavigation = new Role { RoleName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value, }
                };
            }
            return null;
        }
    }
}
