using RN_TaskManager.DAL.Context;
using RN_TaskManager.Models;

namespace RN_TaskManager.DAL.Repositories
{
    public class GroupRepository : BaseRepository<Group>, IGroupRepository
    {
        private readonly RN_TaskManagerContext _context;

        public GroupRepository(RN_TaskManagerContext context) : base(context)
        {
            _context = context;
        }

    }
}