using api_CodeFlow.Model;
using api_CodeFlow.Model.ModelsForAdd;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace api_CodeFlow.Controllers.Product
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImagesController : ControllerBase
    {
        database_codeflowContext dbContext = new database_codeflowContext();
        [HttpPost]
        [Route("AddProductImage/{product_id}")]
        [Authorize(Roles = "Издатель")]
        public async Task<ActionResult<ProductImage>> AddImage(int product_id, IFormFile image)
        {
            long maxFileSize = 15 * 1024 * 1024;
            if (image.Length > maxFileSize)
            {
                return BadRequest("Файл слишком большой");
            }
            Model.Product product = dbContext.Products.Include(x => x.ProductPublisher).FirstOrDefault(x => x.ProductId == product_id);
            if (product == null)
            {
                return BadRequest("Товар не найден");
            }

            /*         
            int product_image_id = 1;
            if (dbContext.ProductImages.Count() != 0)
            {
                product_image_id = dbContext.ProductImages.Max(x => x.ProductImageId) + 1;
            }*/

            var fileName = Path.GetFileName(image.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "publishers", product.ProductPublisher.PublisherName, product.ProductName, "images", fileName);
            using (var stream = System.IO.File.Create(filePath))
            {
                await image.CopyToAsync(stream);
            }

            dbContext.Add(new ProductImage
            {
                ProductId = product_id,
                ProductImagePath = fileName,
            });
            dbContext.SaveChanges();
            return new ObjectResult("Изображение добавлено") { StatusCode = StatusCodes.Status201Created };
        }
        [HttpGet]
        public async Task<ActionResult<List<ProductImage>>> GetImages()
        {
            List<ProductImage> productImages = dbContext.ProductImages.ToList();
            if (productImages == null)
            {
                return NotFound("Не найдено");
            }

            return Ok(productImages);

        }
        [HttpDelete]
        [Route("{image_id}")]
        [Authorize(Roles = "Издатель,Модератор контента")]
        public async Task<ActionResult<List<ProductImage>>> DeleteImage(int image_id)
        {
            ProductImage image = dbContext.ProductImages.Include(x => x.Product).FirstOrDefault(x => x.ProductImageId == image_id);
            if (image == null)
            {
                return BadRequest("Не найдено");
            }

            Model.Product product = dbContext.Products.Find(image.Product.ProductId);
            if (product == null)
            {
                return BadRequest("Товар не найден");
            }
            Model.User currentUser = GetCurrectUser();
            if (currentUser.UserRoleNavigation.RoleName != "Издатель" || currentUser.UserRoleNavigation.RoleName != "Модератор контента")
            {
                return BadRequest("Нет прав доступа");
            }
            Model.User user = dbContext.Users.Find(currentUser.UserId);
            if (user == null)
            {
                return BadRequest("Пользователь не найден");
            }

            if (currentUser.UserRoleNavigation.RoleName == "Издатель")
            {
                Publisher publisher = dbContext.Publishers.FirstOrDefault(x => x.PublisherUserId == user.UserId);
                if (publisher == null)
                {
                    return BadRequest("Издатель не найден");
                }
                if (product.ProductPublisherId != publisher.PublisherId)
                {
                    return BadRequest("Нет прав доступа к товару");
                }
            }
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "publishers", product.ProductPublisher.PublisherName, product.ProductName, "images", image.ProductImagePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                dbContext.ProductImages.Remove(image);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           

            return Ok("Изображение удалено");
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

