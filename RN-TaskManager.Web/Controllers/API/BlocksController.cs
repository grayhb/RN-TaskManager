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
    public class BlocksController : ControllerBase
    {
        private readonly IBlockRepository _blockRepository;

        public BlocksController(IBlockRepository blockRepository)
        {
            _blockRepository = blockRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IList<Block>>> GetItems()
        {
            try
            {
                var items = await _blockRepository.FindAsync(e => !e.Deleted);
                return items.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Block>> GetItem(int id)
        {
            var item = await _blockRepository.FindByIdAsync(id);

            if (item == null)
                return NotFound();
            else
                return item;
        }

        [HttpPost]
        public async Task<ActionResult<Block>> CreateItem([FromForm] Block item)
        {
            try
            {
                var existItems = await _blockRepository
                .FindAsync(e => e.BlockName.ToLower().Equals(item.BlockName.ToLower()) && !e.Deleted);

                if (existItems.Count > 0)
                    return BadRequest("Статус с таким наименованием уже существует");

                if (item.BlockId > 0)
                    return BadRequest("Идентификатор записи должен быть равен 0");

                await _blockRepository.CreateAsync(item);

                return item;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<Block>> UpdateItem([FromForm] Block item)
        {
            try
            {
                var existItem = await _blockRepository.FindByIdAsync(item.BlockId);

                if (existItem == null)
                    return NotFound();

                existItem.BlockName = item.BlockName;

                await _blockRepository.EditAsync(existItem);
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
                var item = await _blockRepository.FindByIdAsync(id);

                if (item == null)
                    return NotFound();
                else
                {
                    item.Deleted = true;

                    await _blockRepository.EditAsync(item);
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
