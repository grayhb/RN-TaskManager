using RN_TaskManager.DAL.Context;
using RN_TaskManager.Models;

namespace RN_TaskManager.DAL.Repositories
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        private readonly RN_TaskManagerContext _context;

        public ProjectRepository(RN_TaskManagerContext context) : base(context)
        {
            _context = context;
        }

    }
}
