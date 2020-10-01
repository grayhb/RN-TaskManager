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
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly IUserRepository _userRepository;


        public ProjectsController(IProjectRepository projectRepository, IProjectTaskRepository projectTaskRepository, IUserRepository userRepository)
        {
            _projectRepository = projectRepository;
            _projectTaskRepository = projectTaskRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IList<Project>>> GetItems()
        {
            try
            {
                var items = await _projectRepository.GetProjectsAsync();
                return items.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetItem(int id)
        {
            try
            {
                var item = await _projectRepository.GetProjectByIdAsync(id);

                if (item == null)
                    return NotFound();
                else
                    return item;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Project>> CreateItem([FromForm] Project item)
        {
            try
            {
                var existItems = await _projectRepository
                .FindAsync(e => e.ProjectName.ToLower().Equals(item.ProjectName.ToLower()) && !e.Deleted);

                if (existItems.Count > 0)
                    return BadRequest("Проект с таким наименованием уже существует");

                if (item.ProjectId > 0)
                    return BadRequest("Идентификатор записи должен быть равен 0");

                if (item.UserId > 0)
                    item.Responsible = await _userRepository.FindByIdAsync(item.UserId.Value);
                else
                    return BadRequest("Не указан Ответственный");


                await _projectRepository.CreateAsync(item);

                return item;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<Project>> UpdateItem([FromForm] Project item)
        {
            try
            {
                var existItem = await _projectRepository.FindByIdAsync(item.ProjectId);

                if (existItem == null)
                    return NotFound();

                if (item.UserId > 0)
                {
                    existItem.Responsible = await _userRepository.FindByIdAsync(item.UserId.Value);
                    existItem.UserId = item.UserId;
                }

                existItem.ProjectName = item.ProjectName;
                existItem.ProjectDescription = item.ProjectDescription;
                existItem.ProjectImportance = item.ProjectImportance;

                await _projectRepository.EditAsync(existItem);

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
                var item = await _projectRepository.FindByIdAsync(id);

                if (item == null)
                    return NotFound();
                else
                {
                    // удалить все связанные записи...
                    var tasks = await _projectTaskRepository.FindAsync(e => e.ProjectId.Equals(id) && !e.Deleted);
                    foreach (var task in tasks)
                        task.Deleted = true;
                    
                    item.Deleted = true;
                    await _projectRepository.EditAsync(item);

                    if (tasks.Count > 0)
                        await _projectTaskRepository.EditAsync(tasks.ToList());

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
