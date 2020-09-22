using Microsoft.EntityFrameworkCore;
using RN_TaskManager.DAL.Context;
using RN_TaskManager.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RN_TaskManager.DAL.Repositories
{
    public class ProjectTaskPerformerRepository : BaseRepository<ProjectTaskPerformer>, IProjectTaskPerformerRepository
    {
        private readonly RN_TaskManagerContext _context;

        public ProjectTaskPerformerRepository(RN_TaskManagerContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IList<ProjectTaskPerformer>> GetItemsByGroupIdAsync(int projectTaskId)
        {
            return await _context.ProjectTaskPerformers
                .Where(e => !e.Deleted && e.ProjectTaskId.Equals(projectTaskId))
                .ToListAsync();
        }
    }
}
