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
    public class ProjectTaskPerformersController : ControllerBase
    {
        private readonly IProjectTaskPerformerRepository _projectTaskPerformerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectTaskRepository _projectTaskRepository;

        public ProjectTaskPerformersController(IProjectTaskPerformerRepository projectTaskPerformerRepository, IUserRepository userRepository, IProjectTaskRepository projectTaskRepository)
        {
            _projectTaskPerformerRepository = projectTaskPerformerRepository;
            _userRepository = userRepository;
            _projectTaskRepository = projectTaskRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IList<ProjectTaskPerformer>>> GetItems()
        {
            try
            {
                var items = await _projectTaskPerformerRepository.FindAsync(e => !e.Deleted);
                return items.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("task/{taskId}")]
        public async Task<ActionResult<IList<ProjectTaskPerformer>>> GetItemsByTaskId(int taskId)
        {
            try
            {
                var items = await _projectTaskPerformerRepository.FindAsync(e => !e.Deleted && e.ProjectTaskId.Equals(taskId));
                return items.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectTaskPerformer>> GetItem(int id)
        {
            var item = await _projectTaskPerformerRepository.FindByIdAsync(id);

            if (item == null)
                return NotFound();
            else
                return item;
        }

        [HttpPost]
        public async Task<ActionResult<ProjectTaskPerformer>> CreateItem([FromForm] ProjectTaskPerformer item)
        {
            try
            {
                if (item.ProjectTaskPerformerId > 0)
                    return BadRequest("Идентификатор записи должен быть равен 0");

                if (item.ProjectTaskId == 0)
                    return BadRequest("Идентификатор задачи не должен быть равен 0");

                if (item.UserId == 0)
                    return BadRequest("Идентификатор пользователя не должен быть равен 0");

                var existItems = await _projectTaskPerformerRepository
                .FindAsync(e => e.UserId.Equals(item.UserId) && e.ProjectTaskId.Equals(item.ProjectTaskId) && !e.Deleted);

                if (existItems.Count > 0)
                    return BadRequest("Указанный исполнитель уже назначен на эту задачу");

                var user = await _userRepository.FindByIdAsync(item.UserId);

                if (user == null)
                    return BadRequest("Пользователь не найден");


                var task = await _projectTaskRepository.FindByIdAsync(item.UserId);

                if (task == null)
                    return BadRequest("Задача не найдена");

                item.User = user;
                item.Task = task;

                await _projectTaskPerformerRepository.CreateAsync(item);

                return item;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<ProjectTaskPerformer>> UpdateItem([FromForm] ProjectTaskPerformer item)
        {
            try
            {
                var existItem = await _projectTaskPerformerRepository.FindByIdAsync(item.ProjectTaskPerformerId);

                if (existItem == null)
                    return NotFound();

                var user = await _userRepository.FindByIdAsync(item.UserId);

                if (user == null)
                    return BadRequest("Пользователь не найден");


                var task = await _projectTaskRepository.FindByIdAsync(item.UserId);

                if (task == null)
                    return BadRequest("Задача не найдена");

                existItem.User = user;
                existItem.Task = task;

                await _projectTaskPerformerRepository.EditAsync(existItem);
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
                var item = await _projectTaskPerformerRepository.FindByIdAsync(id);

                if (item == null)
                    return NotFound();
                else
                {
                    item.Deleted = true;

                    await _projectTaskPerformerRepository.EditAsync(item);
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
