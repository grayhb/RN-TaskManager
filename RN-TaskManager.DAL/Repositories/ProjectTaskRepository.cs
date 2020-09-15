using RN_TaskManager.DAL.Context;
using RN_TaskManager.Models;

namespace RN_TaskManager.DAL.Repositories
{
    public class ProjectTaskRepository : BaseRepository<ProjectTask>, IProjectTaskRepository
    {
        private readonly RN_TaskManagerContext _context;

        public ProjectTaskRepository(RN_TaskManagerContext context) : base(context)
        {
            _context = context;
        }

    }
}