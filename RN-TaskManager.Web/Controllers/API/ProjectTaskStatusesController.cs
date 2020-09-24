using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RN_TaskManager.DAL.Repositories;
using RN_TaskManager.Models;

namespace RN_TaskManager.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectTaskStatusesController : ControllerBase
    {
        private readonly IProjectTaskStatusRepository _projectTaskStatusRepository;

        public ProjectTaskStatusesController(IProjectTaskStatusRepository projectTaskStatusRepository)
        {
            _projectTaskStatusRepository = projectTaskStatusRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IList<ProjectTaskStatus>>> GetItems()
        {
            try
            {
                var items = await _projectTaskStatusRepository.FindAsync(e => !e.Deleted);
                return items.OrderBy(e => e.Order).ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectTaskStatus>> GetItem(int id)
        {
            var item = await _projectTaskStatusRepository.FindByIdAsync(id);

            if (item == null)
                return NotFound();
            else
                return item;
        }

        [HttpPost]
        public async Task<ActionResult<ProjectTaskStatus>> CreateItem([FromForm] ProjectTaskStatus item)
        {
            try
            {
                var existItems = await _projectTaskStatusRepository
                .FindAsync(e => e.StatusName.ToLower().Equals(item.StatusName.ToLower()) && !e.Deleted);

                if (existItems.Count > 0)
                    return BadRequest("Статус с таким наименованием уже существует");

                if (item.ProjectTaskStatusId > 0)
                    return BadRequest("Идентификатор записи должен быть равен 0");

                await _projectTaskStatusRepository.CreateAsync(item);

                return item;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<ProjectTaskStatus>> UpdateItem([FromForm] ProjectTaskStatus item)
        {
            try
            {
                var existItem = await _projectTaskStatusRepository.FindByIdAsync(item.ProjectTaskStatusId);

                if (existItem == null)
                    return NotFound();

                existItem.StatusName = item.StatusName;
                existItem.StatusColor = item.StatusColor;
                existItem.Order = item.Order;

                await _projectTaskStatusRepository.EditAsync(existItem);
                return existItem;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var item = await _projectTaskStatusRepository.FindByIdAsync(id);

                if (item == null)
                    return NotFound();
                else
                {
                    item.Deleted = true;

                    await _projectTaskStatusRepository.EditAsync(item);
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
