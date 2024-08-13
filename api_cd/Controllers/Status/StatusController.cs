using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api_CodeFlow.Controllers.Status
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Model.Status>>> GetStatuses()
        {
            List<Model.Status> status_list = dbContext.Context.Statuses.ToList();
            if (status_list.Count == 0)
            {
                return NotFound("Статусы не найдены");
            }
            return Ok(status_list);
        }

        [HttpPost]
        [Route("AddStatus")]
        [Authorize(Roles = "Куратор контента")]
        public async Task<ActionResult<Model.Status>> AddStatus(string status_name)
        {
            if (dbContext.Context.Statuses.FirstOrDefault(x => x.StatusName == status_name) != null)
            {
                return Conflict("Статус уже существует");
            }
            dbContext.Context.Statuses.Add(new Model.Status
            {
                StatusId = dbContext.Context.Statuses.Max(x => x.StatusId) + 1,
                StatusName = status_name,
            });
            dbContext.Context.SaveChanges();
            return new ObjectResult("Добавлено") { StatusCode = StatusCodes.Status201Created };
        }

        [HttpGet]
        [Route("ProductStatuses")]
        public async Task<ActionResult<List<Model.Status>>> GetProductStatuses()
        {
            List<Model.Status> status_list = dbContext.Context.Statuses.Where(x => x.StatusType == 5).ToList();
            if (status_list.Count == 0)
            {
                return NotFound("Статусы не найдены");
            }
            return Ok(status_list);
        }
        [HttpGet]
        [Route("UpdateStatuses")]
        public async Task<ActionResult<List<Model.Status>>> GetUpdateStatuses()
        {
            List<Model.Status> status_list = dbContext.Context.Statuses.Where(x => x.StatusType == 2).ToList();
            if (status_list.Count == 0)
            {
                return NotFound("Статусы не найдены");
            }
            return Ok(status_list);
        }
    }
}
