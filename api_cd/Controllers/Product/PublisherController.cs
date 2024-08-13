using api_CodeFlow.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace api_CodeFlow.Controllers.Product
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly database_codeflowContext dbContext = new database_codeflowContext();
        [HttpGet]
        public async Task<ActionResult<List<Publisher>>> GetPublishers()
        {
            List<Publisher> publisherList = dbContext.Publishers.ToList();
            if (publisherList.Count == 0)
            {
                return NotFound("Не найдено");
            }
            return Ok(publisherList);
        }

        [HttpPost]
        [Route("AddPublisher")]
        [Authorize(Roles = "Куратор контента")]
        public async Task<ActionResult<Publisher>> AddPublisher(string publisher_name)
        {
            if (dbContext.Publishers.FirstOrDefault(x => x.PublisherName == publisher_name) != null)
            {
                return base.Conflict("Издатель уже существует");
            }

            int id = 1;
            if (dbContext.Publishers.Max(x => x.PublisherId) != 0)
            {
                id = dbContext.Publishers.Max(x => x.PublisherId) + 1;
            }
            dbContext.Publishers.Add(new Publisher
            {
                PublisherId = id,
                PublisherName = publisher_name,

            });
            dbContext.SaveChanges();
            return Ok("Добавлено");
        }

        [HttpGet]
        [Route("PublishedProducts/{publisher_id}")]
        public async Task<ActionResult<List<Model.Product>>> GetPublisherProducts(int publisher_id)
        {
            List<Model.Product> publisher_products = dbContext.Products.Where(x => x.ProductPublisherId == publisher_id)
                            .Include(p => p.ProductPublisher)
                            .Include(p => p.ProductStatus)
                            .Include(p => p.ProductGenres).ToList();

            if (publisher_products.Count == 0)
            {
                return NoContent();
            }
            return Ok(publisher_products);
        }
        [HttpGet]
        [Route("PublishedProducts")]
        [Authorize(Roles = "Издатель")]
        public async Task<ActionResult<List<Model.Product>>> GetPublishedProducts()
        {
            Model.User user = GetCurrectUser();
            if (user == null)
            {
                return BadRequest("Пользователь не найден");
            }
            Publisher publisher = dbContext.Publishers.FirstOrDefault(x => x.PublisherUserId == user.UserId);
            if (publisher == null)
            {
                return BadRequest("Издатель не найден");
            }
            List<Model.Product> publisher_products = dbContext.Products.Where(x => x.ProductPublisherId == publisher.PublisherId)
                            .Include(p => p.ProductPublisher)
                            .Include(p => p.ProductStatus)
                            .Include(p => p.ProductGenres)
                            .Include(p => p.PurchaseLists)
                            .ThenInclude(p => p.Purchase).ToList();

            if (publisher_products.Count == 0)
            {
                return NoContent();
            }
            return Ok(publisher_products);
        }

        [HttpGet]
        [Route("CurrentPublisher")]
        [Authorize(Roles = "Издатель")]
        public async Task<ActionResult<Publisher>> GetCurrentPublisherInfo()
        {
            Model.User user = GetCurrectUser();
            if (user == null)
            {
                return BadRequest("Пользователь не найден");
            }
            Publisher publisher = dbContext.Publishers.Include(x => x.Products).FirstOrDefault(x => x.PublisherUserId == user.UserId);
            if (publisher == null)
            {
                return BadRequest("Издатель не найден");
            }
            return publisher;
        }
        [HttpGet]
        [Route("GetReport")]
        [Authorize]
        public async Task<IActionResult> GetReport(List<int> products_range, DateTime from, DateTime too)
        {

            return Ok();
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
