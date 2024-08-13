using api_CodeFlow.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api_CodeFlow.Controllers.Product
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        database_codeflowContext dbContext = new database_codeflowContext();
        [HttpGet]
        public async Task<ActionResult<List<Genre>>> GetGenreList()
        {
            List<Genre> genres = dbContext.Genres.ToList();
            if (genres.Count == 0)
            {
                return NotFound();
            }
            return Ok(genres);
        }

    }
}
