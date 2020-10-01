using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RN_TaskManager.DAL.Repositories;
using RN_TaskManager.Models;

namespace RN_TaskManager.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskTypesController : ControllerBase
    {
        private readonly ITaskTypeRepository _taskStatusRepository;

        public TaskTypesController(ITaskTypeRepository taskStatusRepository)
        {
            _taskStatusRepository = taskStatusRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IList<TaskType>>> GetItems()
        {
            try
            {
                var items = await _taskStatusRepository.FindAsync(e => !e.Deleted);
                return items.OrderBy(e => e.Order).ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskType>> GetItem(int id)
        {
            var item = await _taskStatusRepository.FindByIdAsync(id);

            if (item == null)
                return NotFound();
            else
                return item;
        }

        [HttpPost]
        public async Task<ActionResult<TaskType>> CreateItem([FromForm] TaskType item)
        {
            try
            {
                var existItems = await _taskStatusRepository
                .FindAsync(e => e.TaskTypeName.ToLower().Equals(item.TaskTypeName.ToLower()) && !e.Deleted);

                if (existItems.Count > 0)
                    return BadRequest("Статус с таким наименованием уже существует");

                if (item.TaskTypeId > 0)
                    return BadRequest("Идентификатор записи должен быть равен 0");

                await _taskStatusRepository.CreateAsync(item);

                return item;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<TaskType>> UpdateItem([FromForm] TaskType item)
        {
            try
            {
                var existItem = await _taskStatusRepository.FindByIdAsync(item.TaskTypeId);

                if (existItem == null)
                    return NotFound();

                existItem.TaskTypeName = item.TaskTypeName;
                existItem.Note = item.Note;
                existItem.Order = item.Order;

                await _taskStatusRepository.EditAsync(existItem);
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
                var item = await _taskStatusRepository.FindByIdAsync(id);

                if (item == null)
                    return NotFound();
                else
                {
                    item.Deleted = true;

                    await _taskStatusRepository.EditAsync(item);
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
