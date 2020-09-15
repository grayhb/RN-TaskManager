using RN_TaskManager.DAL.Context;
using RN_TaskManager.Models;

namespace RN_TaskManager.DAL.Repositories
{
    public class ProjectTaskTypeRepository : BaseRepository<ProjectTaskType>, IProjectTaskTypeRepository
    {
        private readonly RN_TaskManagerContext _context;

        public ProjectTaskTypeRepository(RN_TaskManagerContext context) : base(context)
        {
            _context = context;
        }

    }
}
