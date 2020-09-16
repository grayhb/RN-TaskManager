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
    public class GroupsController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;

        public GroupsController(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        [HttpGet]
        public async Task<IList<Group>> GetGroups()
        {
            return await _groupRepository.FindAsync(e => !e.Deleted);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetGroup(int id)
        {
            var item = await _groupRepository.FindByIdAsync(id);

            if (item == null)
                return NotFound();
            else
                return item;
        }

        [HttpPost]
        public async Task<ActionResult<Group>> CreateGroup([FromForm] Group group)
        {
            var existItems = await _groupRepository
                .FindAsync(e => e.GroupName.ToLower().Equals(group.GroupName.ToLower()) && !e.Deleted 
                || e.GroupNumber.ToLower().Equals(group.GroupNumber.ToLower()) && !e.Deleted);

            if (existItems.Count > 0)
                return BadRequest("Группа с таким номером или названием уже существует");

            Group item = new Group() {
                GroupName = group.GroupName,
                GroupNumber = group.GroupNumber
            };

            try
            {
                await _groupRepository.CreateAsync(item);
                return item;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<Group>> UpdateGroup([FromForm] Group group)
        {
            var existItem = await _groupRepository.FindByIdAsync(group.GroupId);

            if (existItem == null)
                return NotFound();

            existItem.GroupName = group.GroupName;
            existItem.GroupNumber = group.GroupNumber;

            try
            {
                await _groupRepository.EditAsync(existItem);
                return existItem;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var item = await _groupRepository.FindByIdAsync(id);

            if (item == null)
                return NotFound();
            else
            {
                item.Deleted = true;

                try
                {
                    await _groupRepository.EditAsync(item);
                    return NoContent();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}
