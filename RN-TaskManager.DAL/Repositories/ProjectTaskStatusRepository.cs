using RN_TaskManager.DAL.Context;
using RN_TaskManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RN_TaskManager.DAL.Repositories
{
    public class ProjectTaskStatusRepository : BaseRepository<ProjectTaskStatus>, IProjectTaskStatusRepository
    {
        private readonly RN_TaskManagerContext _context;

        public ProjectTaskStatusRepository(RN_TaskManagerContext context) : base(context)
        {
            _context = context;
        }

    }
}
