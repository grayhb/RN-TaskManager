using RN_TaskManager.DAL.Context;
using RN_TaskManager.Models;

namespace RN_TaskManager.DAL.Repositories
{
    public class BlockRepository : BaseRepository<Block>, IBlockRepository
    {
        private readonly RN_TaskManagerContext _context;

        public BlockRepository(RN_TaskManagerContext context) : base(context)
        {
            _context = context;
        }
    }
}
