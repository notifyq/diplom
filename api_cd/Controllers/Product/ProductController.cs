using api_CodeFlow.Extentions;
using api_CodeFlow.Model;
using api_CodeFlow.Model.ModelsForAdd;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using System.Security.Claims;

namespace api_CodeFlow.Controllers.Product
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        database_codeflowContext dbContext = new database_codeflowContext(); 
        [HttpGet]
        [Route("ProductList")]
        public async Task<ActionResult<List<Model.Product>>> GetProductList()
        {
            List<Model.Product> product_list = dbContext.Products
                            .Include(p => p.ProductPublisher)
                            .Include(p => p.ProductStatus)
                            .Include(p => p.ProductGenres).ThenInclude(pg => pg.Genre)
                            .ToList();
            if (product_list.Count == 0)
            {
                return NotFound();
            }

            return product_list;
        }
        [HttpPost]
        [Authorize(Roles = "Издатель")]
        public async Task<IActionResult> AddNewProduct(Model.ProductAdd product)
        {
            if (product == null)
            {
                return BadRequest("Товар не найден");
            }
            Model.User currentUser = GetCurrectUser();
            if (currentUser == null)
            {
                return BadRequest("Пользователь не найден");
            }

            Publisher publisher = dbContext.Publishers.FirstOrDefault(x => x.PublisherUserId == currentUser.UserId);

            if (publisher == null)
            {
                return BadRequest("Пользователь не является издателем");
            }

            Model.Product new_product = new Model.Product()
            {
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductPrice = product.ProductPrice,
                ProductStatusId = 14,
                ProductPublisherId = publisher.PublisherId,
            };
            dbContext.Products.Add(new_product);
            dbContext.SaveChanges();


            foreach (var genreId in product.GenreIds)
            {
                var genre = dbContext.Genres.Find(genreId);
                if (genre != null)
                {
                    new_product.ProductGenres.Add(new ProductGenre { GenreId = genreId, ProductId = new_product.ProductId });
                }
            }

            dbContext.SaveChanges();
            return Ok();

        }
        [HttpGet]
        [Route("ProductListByGenre/{genre_id}")]
        public async Task<ActionResult<List<Model.Product>>> GetProductListByGenre(int genre_id)
        {
            List<Model.Product> product_list = dbContext.Products
                            .Include(p => p.ProductPublisher)
                            .Include(p => p.ProductStatus)
                            .Include(p => p.ProductGenres).ThenInclude(pg => pg.Genre)
                            .ToList();

            product_list = product_list.Where(x => x.ProductGenres.Any(x => x.GenreId == genre_id)).ToList();

            if (product_list.Count == 0)
            {
                return NotFound();
            }

            return product_list;
        }
        [HttpGet]
        [Route("ProductListByName/{product_name}")]
        public async Task<ActionResult<List<Model.Product>>> GetProductListByName(string product_name)
        {
            List<Model.Product> product_list = dbContext.Products
                            .Include(p => p.ProductPublisher)
                            .Include(p => p.ProductStatus)
                            .Include(p => p.ProductGenres).ThenInclude(pg => pg.Genre)
                            .Where(x => x.ProductName.Contains(product_name)).ToList();
            if (product_list.Count == 0)
            {
                return NotFound();
            }

            return product_list;
        }

        [HttpGet("{product_id}")]
        public async Task<ActionResult> GetProduct(int product_id)
        {
            var product = await dbContext.Products
                            .Include(p => p.ProductPublisher)
                            .Include(p => p.ProductStatus)
                            .Include(p => p.ProductGenres)
                            .ThenInclude(x => x.Genre)
                            .FirstOrDefaultAsync(p => p.ProductId == product_id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        public class ProductData
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductDescription { get; set; }
            public decimal ProductPrice { get; set; }
            public List<int> GenreIds { get; set; }

        }
        [HttpPut]
        [Route("UpdateProductInfo")]
        [Authorize(Roles = "Издатель")]
        public async Task<ActionResult> UpdateProductInfo(ProductData productData)
        {
            try
            {
                Model.Product _product = dbContext.Products.Find(productData.ProductId);
                if (_product == null)
                {
                    return BadRequest("Товар не найден");
                }
                Model.User currentUser = GetCurrectUser();
                if (currentUser.UserRoleNavigation.RoleName != "Издатель")
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
                    if (_product.ProductPublisherId != publisher.PublisherId)
                    {
                        return BadRequest("Нет прав доступа к товару");
                    }
                }

                _product.ProductId = productData.ProductId;
                _product.ProductName = productData.ProductName;
                _product.ProductPrice = productData.ProductPrice;
                _product.ProductDescription = productData.ProductDescription;

                var currentGenres = dbContext.ProductGenres.Where(pg => pg.ProductId == productData.ProductId).ToList();
                dbContext.ProductGenres.RemoveRange(currentGenres);
                await dbContext.SaveChangesAsync();


                foreach (var genreId in productData.GenreIds)
                {
                    var genre = dbContext.Genres.Find(genreId);
                    if (genre != null)
                    {
                        _product.ProductGenres.Add(new ProductGenre { GenreId = genreId, ProductId = _product.ProductId });
                    }
                }

                await dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }

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

        [HttpGet("LatestVersion/{product_id}")]
        public async Task<IActionResult> GetLatestVersion(int product_id)
        {
            ProductUpdate latestUpdate = await dbContext.ProductUpdates
                .Include(x => x.UpdateStatusNavigation)
                .Where(x => x.ProductId == product_id && x.UpdateStatus == 7)
                .OrderByDescending(x => x.UpdateDate)
                .FirstOrDefaultAsync();

            if (latestUpdate == null)
            {
                return NotFound("Версия не найдена");
            }

            return Ok(latestUpdate);
        }
        [HttpDelete]
        [Route("Updates/{update_id}")]
/*        [Authorize(Roles = "Издатель")]
*/        public async Task<ActionResult> DeleteUpdate(int update_id)
        {
            ProductUpdate productUpdate = dbContext.ProductUpdates.Include(x => x.Product).ThenInclude(x => x.ProductPublisher).FirstOrDefault(x => x.ProductUpdateId == update_id);
            if (productUpdate == null)
            {
                return BadRequest("Обновление не найдено");
            }
            /*            Model.User currentUser = GetCurrectUser();
                        if (currentUser.UserRoleNavigation.RoleName != "Издатель")
                        {
                            return BadRequest("Нет прав доступа");
                        }
                        Model.User user = dbContext.Users.Find(currentUser.UserId);
                        if (user == null)
                        {
                            return BadRequest("Пользователь не найден");
                        }
                        Publisher publisher = dbContext.Publishers.FirstOrDefault(x => x.PublisherUserId == user.UserId);
                        if (publisher == null)
                        {
                            return BadRequest("Издатель не найден");
                        }
                        if (productUpdate.Product.ProductPublisherId != publisher.PublisherId)
                        {
                            return BadRequest("Нет прав доступа к товару");
                        }
                       */

            try
            {
                dbContext.ProductUpdates.Remove(productUpdate);
                dbContext.SaveChanges();
                string projectDirectory = System.IO.Directory.GetCurrentDirectory();
                string path = $"{projectDirectory}/publishers/{productUpdate.Product.ProductPublisher.PublisherName}/{productUpdate.Product.ProductName}/updates/{productUpdate.ProductVersion}";
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpPut]
        [Route("SetForSale/{product_id}")]
        [Authorize(Roles = "Модератор контента")]
        public async Task<ActionResult> SetForSale(int product_id)
        {
            Model.Product product = dbContext.Products.Find(product_id);
            if (product == null)
            {
                return BadRequest("Товар не найден");
            }
            Model.User currentUser = GetCurrectUser();
            if (currentUser.UserRoleNavigation.RoleName != "Модератор контента")
            {
                return BadRequest("Нет прав доступа");
            }
            Model.User user = dbContext.Users.Find(currentUser.UserId);
            if (user == null)
            {
                return BadRequest("Пользователь не найден");
            }
            product.ProductStatusId = 15;
            dbContext.SaveChanges();
            return Ok("Теперь товар в продаже");
        }

        [HttpPut]
        [Route("SetNotForSale/{product_id}")]
        [Authorize(Roles = "Модератор контента")]
        public async Task<ActionResult> SetNotForSale(int product_id)
        {
            Model.Product product = dbContext.Products.Find(product_id);
            if (product == null)
            {
                return BadRequest("Товар не найден");
            }
            Model.User currentUser = GetCurrectUser();
            if (currentUser.UserRoleNavigation.RoleName != "Модератор контента")
            {
                return BadRequest("Нет прав доступа");
            }
            Model.User user = dbContext.Users.Find(currentUser.UserId);
            if (user == null)
            {
                return BadRequest("Пользователь не найден");
            }
            product.ProductStatusId = 14;
            dbContext.SaveChanges();
            return Ok("Теперь товар не в продаже");
        }

        [HttpPut]
        [Route("SetArchive/{product_id}")]
        [Authorize(Roles = "Издатель,Модератор контента")]
        public async Task<ActionResult> SetArchive(int product_id)
        {
            Model.Product product = dbContext.Products.Find(product_id);
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

            product.ProductStatusId = 16;
            dbContext.SaveChanges();

            return Ok("Товар теперь не в продаже");
        }
        [HttpGet]
        [Route("Updates/{product_id}")]
        public async Task<ActionResult<List<ProductUpdate>>> GetProductUpdates(int product_id)
        {
            List<ProductUpdate> productUpdates = dbContext.ProductUpdates.Include(x => x.UpdateStatusNavigation).Where(x => x.ProductId == product_id).ToList();
            if (productUpdates.Count == 0)
            {
                return NoContent();
            }
            return productUpdates;
        }
        [HttpPut]
        [Authorize(Roles = "Модератор контента")]
        [Route("Updates/{update_id}/{status_id}")]
        public async Task<ActionResult<List<ProductUpdate>>> GetProductUpdates(int update_id, int status_id)
        {
            ProductUpdate productUpdate = dbContext.ProductUpdates.FirstOrDefault(x => x.ProductUpdateId == update_id);
            if (productUpdate == null)
            {
                return BadRequest();
            }
            List<Model.Status> update_statuses = dbContext.Statuses.Where(x => x.StatusType == 2).ToList();

            if (!update_statuses.Any(s => s.StatusId == status_id))
            {
                return BadRequest("Данный status_id не найден в списке возможных статусов.");
            }

            productUpdate.UpdateStatus = status_id;
            dbContext.SaveChanges();

            return Ok();
        }




        [HttpGet("Download/{product_id}/{updateVersion?}")]
        public async Task<IActionResult> LoadProduct(int product_id, CancellationToken cancellationToken, string updateVersion = "null")
        {
            Console.WriteLine("Начало загрузки продукта: " + product_id.ToString() + "\n Версия: " + updateVersion);
            var product = await dbContext.Products.Include(p => p.ProductPublisher)
                            .FirstOrDefaultAsync(p => p.ProductId == product_id);
            if (product == null)
            {
                return BadRequest("Товар не найден");
            }
            string projectDirectory = System.IO.Directory.GetCurrentDirectory();
            string path = $"{projectDirectory}/publishers/{product.ProductPublisher.PublisherName}/{product.ProductName}/updates/{updateVersion}";

            List<ProductUpdate> updates = dbContext.ProductUpdates.Where(x => x.ProductId == product_id).OrderBy(x => x.UpdateDate).ToList();
            ProductUpdate productUpdate;

            if (updateVersion == "null" && updates.Count > 0)
            {
                path = $"{projectDirectory}/publishers/{product.ProductPublisher.PublisherName}/{product.ProductName}/updates/{updates[0].ProductVersion}";
                productUpdate = updates[0];
            }
            else
            {
                return BadRequest("Нет версий для приложения");
            }
            if (updateVersion != "null" && updates.Count > 0)
            {
                productUpdate = updates.FirstOrDefault(x => x.ProductVersion == updateVersion);
            }
            else if (updateVersion == null)
            {
                return BadRequest("Версия не найдена");
            }
            if (!Directory.Exists(path))
            {
                return NotFound($"Версия '{updateVersion}' не найдена");
            }

            // Путь к временному zip-файлу
            string zipFilePath = $"{path}.zip";

            ProductUpdateModelSend temp = new ProductUpdateModelSend()
            {
                UpdateDate = productUpdate.UpdateDate,
                ProductVersion = productUpdate.ProductVersion,
                ProductId = productUpdate.ProductId,
                ProductUpdateId = productUpdate.ProductUpdateId,
                UpdateStatus = productUpdate.UpdateStatus,
                UpdateStatusNavigation = productUpdate.UpdateStatusNavigation,
            };
            string updateInfoJson = JsonConvert.SerializeObject(temp);

            if (!System.IO.File.Exists(zipFilePath))
            {
                ZipFile.CreateFromDirectory(path, zipFilePath);
            }
            var stream = System.IO.File.OpenRead(zipFilePath);
            FileResult response = new FileStreamResult(stream, GetContentType(zipFilePath)); // Инициализация response

            Console.WriteLine(updateInfoJson);

            Response.Headers.Add("Update-Info", updateInfoJson);

            var rangeHeader = Request.Headers["Range"].ToString();
            if (!string.IsNullOrEmpty(rangeHeader) && rangeHeader.StartsWith("bytes="))
            {
                var range = rangeHeader.Substring(6).Split('-');
                if (range.Length == 2)
                {
                    var start = long.Parse(range[0]);
                    var end = string.IsNullOrEmpty(range[1]) ? stream.Length - 1 : long.Parse(range[1]);

                    response = new PartialFileResult(stream, GetContentType(zipFilePath), start, end);
                }
            }

            return response;

        }
        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

        [HttpPost("Upload/{product_id}")]
        [Authorize(Roles = "Издатель")]
        public async Task<IActionResult> UploadProductFiles(int product_id, string updateVersion)
        {
            if (Request.ContentLength == 0)
            {
                return BadRequest("Файл не был получен");
            }

            Model.Product product = dbContext.Products.Include(x => x.ProductPublisher).FirstOrDefault(x => x.ProductId == product_id);
            if (product == null)
            {
                return BadRequest("Товар не найден");
            }

            string projectDirectory = System.IO.Directory.GetCurrentDirectory();
            string path = $"{projectDirectory}/publishers/{product.ProductPublisher.PublisherName}/{product.ProductName}/updates/{updateVersion}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // Сохранить архив в файловой системе сервера
            var filePath = Path.Combine(path, "file.zip");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Request.Body.CopyToAsync(stream);
            }

            // Извлечь архив
            ZipFile.ExtractToDirectory(filePath, path);

            // Добавить информацию о версии продукта в базу данных
            dbContext.Add(new ProductUpdate
            {
                ProductId = product_id,
                ProductVersion = updateVersion,
                UpdateStatus = 14,
                UpdateDate = DateTime.Now,
            });
            dbContext.SaveChanges();

            System.IO.File.Delete(filePath);
            return new ObjectResult("Версия продукта загружена") { StatusCode = StatusCodes.Status201Created };
        }
            

        [HttpPost]
        [Route("ProductRangeList")]
        public async Task<ActionResult> GetProductFromIdList(List<int> product_id_list)
        {
            product_id_list = product_id_list.Distinct().ToList();

            List<Model.Product> product = dbContext.Products
                            .Include(p => p.ProductPublisher)
                            .Include(p => p.ProductStatus)
                            .Include(p => p.ProductGenres)
                            .Where(p => product_id_list.Contains(p.ProductId) && p.ProductStatusId == 15).ToList();

            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet]
        [Route("Images/{product_id}")]
        public async Task<ActionResult> GetProductImages(int product_id)
        {
            var product = await dbContext.Products
                            .Include(p => p.ProductImages)
                            .Include(p => p.ProductPublisher)
                            .FirstOrDefaultAsync(p => p.ProductId == product_id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product.Images);
        }

        [HttpGet]
        [Route("ImagesWithId/{product_id}")]
        public async Task<ActionResult> GetProductImagesWithId(int product_id)
        {
            var product = await dbContext.Products
                            .Include(p => p.ProductImages)
                            .Include(p => p.ProductPublisher)
                            .FirstOrDefaultAsync(p => p.ProductId == product_id);
            if (product == null)
            {
                return NotFound();
            }

            string projectDirectory = System.IO.Directory.GetCurrentDirectory();

            List<ProductImageModel> productImages = new List<ProductImageModel>();
            foreach (var image in product.ProductImages)
            {
                byte[] imageBytes = System.IO.File.ReadAllBytes($"{projectDirectory}/publishers/{product.ProductPublisher.PublisherName}/{product.ProductName}/images/{image.ProductImagePath}");
                string base64String = Convert.ToBase64String(imageBytes);
                productImages.Add(new ProductImageModel { ImageId = image.ProductImageId, ImageData = base64String });
            }
            return Ok(productImages);
        }

        public class ProductImageModel
        {
            public int ImageId { get; set; }
            public string ImageData { get; set; }
        }

        /*[HttpPost]
        [ActionName("AddProduct")]
        [Route("AddProduct")]
        [Authorize(Roles = "Модератор контента")]
        public async Task<ActionResult<Model.Product>> AddProduct(ProductAdd product)
        {
            if (product == null)
            {
                return BadRequest();
            }
            if (ApplicationContext.Context.Products.Find(product.ProductId) != null)
            {
                return Conflict($"Товар с {product.ProductId} уже сущетсвует");
            }
            switch (product.ProductStatus)
            {
                case 3:
                    product.ProductStatus = 3;
                    break;
                case 4:
                    product.ProductStatus = 4;
                    break;
                default:
                    return BadRequest("Статус товара может быть в продаже или не в продаже");
                    break;
            }

            Product new_product = new Product()
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductDeveloper = product.ProductDeveloper,
                ProductPublisher = product.ProductPublisher,
                ProductPrice = product.ProductPrice,
                ProductStatus = product.ProductStatus,
            };

            ApplicationContext.Context.Add(new_product);
            ApplicationContext.Context.SaveChanges();

            return new ObjectResult("Товар добавлен") { StatusCode = StatusCodes.Status201Created };
        }
        [HttpPut]
        [Route("ChangeStatus")]
        [Authorize(Roles = "Модератор контента")]
        public async Task<ActionResult<Product>> ChangeStatus(int product_id, int status_id)
        {
            Product product = ApplicationContext.Context.Products.Find(product_id);
            if (product == null)
            {
                return BadRequest("Товар не найден");
            }
            switch (status_id)
            {
                case 3:
                    product.ProductStatus = 3;
                    break;
                case 4:
                    product.ProductStatus = 4;
                    break;
                default:
                    return BadRequest("Статус может быть в продаже или не в продаже");
                    break;
            }

            return Ok("Статус товара изменен на " + ApplicationContext.Context.Statuses.Find(status_id).StatusName);
        }*/
    }
}
