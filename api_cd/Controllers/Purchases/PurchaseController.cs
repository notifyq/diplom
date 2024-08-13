using api_CodeFlow.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace api_CodeFlow.Controllers.Purchases
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        database_codeflowContext dbContext = new database_codeflowContext();
        EmailMessageSending emailMessageSending = new EmailMessageSending();

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> BuyProducts(List<int> product_id_list)
        {
            product_id_list = product_id_list.Distinct().ToList();

            List<Model.Product> products = dbContext.Products
                            .Include(p => p.ProductPublisher)
                            .Include(p => p.ProductStatus)
                            .Include(p => p.ProductGenres)
                            .Where(p => product_id_list.Contains(p.ProductId) && p.ProductStatusId == 15).ToList();
            Model.User user = GetCurrectUser();
            user = dbContext.Users.Find(user.UserId);

            if (products == null || user == null)
            {
                return BadRequest();
            }

            if (products.Count == 0)
            {
                return NotFound();
            }

            var purchase = new Purchase()
            {
                UserId = user.UserId,
                PurchaseDate = DateTime.Now,
                PurchaseStatus = 10,
            };
            dbContext.Purchases.Add(purchase);
            dbContext.SaveChanges();
            int id = purchase.PurchasesId;


            foreach (var item in products)
            {
                dbContext.PurchaseLists.Add(new PurchaseList()
                {
                    PurchaseId = id,
                    ProductId = item.ProductId,
                    ProductSpentMoney = item.ProductPrice,
                    Key = GenerateUniqueKeyAsync().Result,
                });
            }
            dbContext.SaveChanges();

            emailMessageSending.SendMailPurchaseInfo(user, products, purchase);

            return Ok();
        }
        [NonAction]
        public async Task<string> GenerateUniqueKeyAsync()
        {
            string key;
            do
            {
                Guid guid = Guid.NewGuid();
                key = guid.ToString();
            }
            while (await KeyExists(key));

            return key;
        }
        [NonAction]
        public async Task<bool> KeyExists(string key)
        {
            return await dbContext.PurchaseLists.AnyAsync(pk => pk.Key == key);
        }
        [NonAction]
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
