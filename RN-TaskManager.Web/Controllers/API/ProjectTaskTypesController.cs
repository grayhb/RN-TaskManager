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
    public class ProjectTaskTypesController : ControllerBase
    {
        private readonly IProjectTaskTypeRepository _projectTaskTypeRepository;
        private readonly IProjectRepository _projectRepository;

        public ProjectTaskTypesController(IProjectTaskTypeRepository projectTaskTypeRepository, IProjectRepository projectRepository)
        {
            _projectTaskTypeRepository = projectTaskTypeRepository;
            _projectRepository = projectRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IList<ProjectTaskType>>> GetItems()
        {
            try
            {
                var items = await _projectTaskTypeRepository.FindAsync(e => !e.Deleted);
                return items.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("p/{projectId}")]
        public async Task<ActionResult<IList<ProjectTaskType>>> GetItemsForProject(int projectId)
        {
            try
            {
                var items = await _projectTaskTypeRepository.FindAsync(e => !e.Deleted && e.ProjectId.Equals(projectId));
                return items.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectTaskType>> GetItem(int id)
        {
            var item = await _projectTaskTypeRepository.FindByIdAsync(id);

            if (item == null)
                return NotFound();
            else
                return item;
        }

        [HttpPost]
        public async Task<ActionResult<ProjectTaskType>> CreateItem([FromForm] ProjectTaskType item)
        {
            try
            {
                if (item.ProjectTaskTypeId > 0)
                    return BadRequest("Идентификатор записи должен быть равен 0");

                if (item.ProjectId == 0)
                    return BadRequest("Идентификатор проекта не должен быть равен 0");

                var existItems = await _projectTaskTypeRepository
                .FindAsync(e => e.ProjectTaskTypeName.ToLower().Equals(item.ProjectTaskTypeName.ToLower()) && e.ProjectId.Equals(item.ProjectId) && !e.Deleted);

                if (existItems.Count > 0)
                    return BadRequest("У данного проекта тип задачи с таким наименованием уже существует");

                var project = await _projectRepository.FindByIdAsync(item.ProjectId);

                if (project == null)
                    return BadRequest("Проект не найден");

                item.Project = project;

                await _projectTaskTypeRepository.CreateAsync(item);

                return item;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<ProjectTaskType>> UpdateItem([FromForm] ProjectTaskType item)
        {
            try
            {
                var existItem = await _projectTaskTypeRepository.FindByIdAsync(item.ProjectTaskTypeId);

                if (existItem == null)
                    return NotFound();

                existItem.ProjectTaskTypeName = item.ProjectTaskTypeName;
                existItem.Order = item.Order;

                await _projectTaskTypeRepository.EditAsync(existItem);
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
                var item = await _projectTaskTypeRepository.FindByIdAsync(id);

                if (item == null)
                    return NotFound();
                else
                {
                    item.Deleted = true;

                    await _projectTaskTypeRepository.EditAsync(item);
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
