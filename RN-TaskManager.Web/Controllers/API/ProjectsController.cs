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

        public ProjectsController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IList<Project>>> GetItems()
        {
            try
            {
                var items = await _projectRepository.FindAsync(e => !e.Deleted);
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
                var item = await _projectRepository.FindByIdAsync(id);

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

                await _projectRepository.CreateAsync(item);

                return item;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<Project>> UpdateItem([FromForm] Project group)
        {
            try
            {
                var existItem = await _projectRepository.FindByIdAsync(group.ProjectId);

                if (existItem == null)
                    return NotFound();

                existItem.ProjectName = group.ProjectName;
                existItem.ProjectImportance = group.ProjectImportance;

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
                    item.Deleted = true;

                    await _projectRepository.EditAsync(item);
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
