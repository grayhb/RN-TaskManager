using RN_TaskManager.DAL.Context;
using RN_TaskManager.Models;

namespace RN_TaskManager.DAL.Repositories
{
    public class TaskTypeRepository : BaseRepository<TaskType>, ITaskTypeRepository
    {
        private readonly RN_TaskManagerContext _context;

        public TaskTypeRepository(RN_TaskManagerContext context) : base(context)
        {
            _context = context;
        }

    }
}
