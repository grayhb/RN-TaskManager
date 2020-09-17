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
        public async Task<ActionResult<IList<Group>>> GetGroups()
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
                var existItems = await _groupRepository
                .FindAsync(e => e.GroupName.ToLower().Equals(item.GroupName.ToLower()) && !e.Deleted
                || e.GroupNumber.ToLower().Equals(item.GroupNumber.ToLower()) && !e.Deleted);

                if (existItems.Count > 0)
                    return BadRequest("Группа с таким номером или названием уже существует");

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
        public async Task<ActionResult<Group>> UpdateItem([FromForm] Group group)
        {
            try
            {
                var existItem = await _groupRepository.FindByIdAsync(group.GroupId);

                if (existItem == null)
                    return NotFound();

                existItem.GroupName = group.GroupName;
                existItem.GroupNumber = group.GroupNumber;


                await _groupRepository.EditAsync(existItem);
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
