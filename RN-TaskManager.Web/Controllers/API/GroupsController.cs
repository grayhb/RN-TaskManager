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
    public class GroupsController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;

        public GroupsController(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IList<Group>>> GetItems()
        {
            try
            {
                var items = await _groupRepository.FindAsync(e => !e.Deleted);
                return items.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetItem(int id)
        {
            try
            {
                var item = await _groupRepository.FindByIdAsync(id);

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
        public async Task<ActionResult<Group>> CreateItem([FromForm] Group item)
        {
            try
            {
                if (item.GroupName == "")
                    return BadRequest("Название группы не должно быть пустым");

                var existItems = await _groupRepository
                .FindAsync(e => e.GroupName.ToLower().Equals(item.GroupName.ToLower()) && !e.Deleted);

                if (existItems.Count > 0)
                    return BadRequest("Группа с таким названием уже существует");

                if (item.GroupId > 0)
                    return BadRequest("Идентификатор записи должен быть равен 0");


                await _groupRepository.CreateAsync(item);
                return item;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<Group>> UpdateItem([FromForm] Group item)
        {
            try
            {
                if (item.GroupName == "")
                    return BadRequest("Название группы не должно быть пустым");

                var existItems = await _groupRepository
                .FindAsync(e => e.GroupName.ToLower().Equals(item.GroupName.ToLower()) && !e.Deleted);

                if (existItems.Count > 0)
                    return BadRequest("Группа с таким названием уже существует");

                var editedItem = await _groupRepository.FindByIdAsync(item.GroupId);

                if (editedItem == null)
                    return NotFound();

                editedItem.GroupName = item.GroupName;
                editedItem.GroupNumber = item.GroupNumber;

                await _groupRepository.EditAsync(editedItem);
                return editedItem;
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
                var item = await _groupRepository.FindByIdAsync(id);

                if (item == null)
                    return NotFound();
                else
                {
                    item.Deleted = true;

                    await _groupRepository.EditAsync(item);
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
