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
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;

        public UsersController(IUserRepository userRepository, IGroupRepository groupRepository)
        {
            _userRepository = userRepository;
            _groupRepository = groupRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IList<User>>> GetItems()
        {
            try
            {
                var items = await _userRepository.GetUsersAsync();
                return items.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetItem(int id)
        {
            var item = await _userRepository.FindByIdAsync(id);

            if (item == null)
                return NotFound();
            else
                return item;
        }

        [HttpGet("g/{groupId}")]
        public async Task<ActionResult<IList<User>>> GetItemsByGroupId(int groupId)
        {
            var items = await _userRepository.GetUsersByGroupIdAsync(groupId);

            return items.ToList();
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateItem([FromForm] User item)
        {
            try
            {
                var existItems = await _userRepository
                .FindAsync(e => e.Login.ToLower().Equals(item.Login.ToLower()) && !e.Deleted);

                if (existItems.Count > 0)
                    return BadRequest("Пользователь с таким логином уже существует");

                if (item.UserId > 0)
                    return BadRequest("Идентификатор записи должен быть равен 0");

                // получение группы если она есть
                if (item.GroupId != null && item.GroupId > 0)
                {
                    var group = await _groupRepository.FindByIdAsync(item.GroupId.Value);
                    item.Group = group;
                }

                item.Guid = Guid.NewGuid();

                await _userRepository.CreateAsync(item);
                return item;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<User>> UpdateItem([FromForm] User item)
        {
            try
            {
                var existItem = await _userRepository.FindByIdAsync(item.UserId);

                if (existItem == null)
                    return NotFound();

                existItem.FirstName = item.FirstName;
                existItem.LastName = item.LastName;
                existItem.Patronymic = item.Patronymic;

                // получение группы если она есть
                if (item.GroupId != null && item.GroupId > 0)
                {
                    var group = await _groupRepository.FindByIdAsync(item.GroupId.Value);
                    existItem.Group = group;
                }

                await _userRepository.EditAsync(existItem);
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
                var item = await _userRepository.FindByIdAsync(id);

                if (item == null)
                    return NotFound();
                else
                {
                    item.Deleted = true;

                    await _userRepository.EditAsync(item);
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
