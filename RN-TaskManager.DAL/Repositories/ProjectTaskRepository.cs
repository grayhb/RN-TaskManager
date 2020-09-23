using Microsoft.EntityFrameworkCore;
using RN_TaskManager.DAL.Context;
using RN_TaskManager.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RN_TaskManager.DAL.Repositories
{
    public class ProjectTaskRepository : BaseRepository<ProjectTask>, IProjectTaskRepository
    {
        private readonly RN_TaskManagerContext _context;

        public ProjectTaskRepository(RN_TaskManagerContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProjectTask> ProjectTaskByIdAsync(int id)
        {
            return await _context.ProjectTasks.Where(e => !e.Deleted && e.ProjectTaskId.Equals(id))
                .Include(e => e.Project)
                .Include(e => e.TaskStatus)
                .Include(e => e.Group)
                .Include(e => e.TaskType)
                .Include(e => e.ProjectTaskPerformers)
                .ThenInclude(e => e.User)
                .SingleOrDefaultAsync();
        }


        public async Task<List<ProjectTask>> ProjectTasksAsync()
        {
            return await _context.ProjectTasks.Where(e => !e.Deleted && !e.Project.Deleted && !e.Group.Deleted && !e.TaskStatus.Deleted && !e.TaskStatus.Deleted)
                .Include(e => e.Project)
                .Include(e => e.TaskStatus)
                .Include(e => e.Group)
                .Include(e => e.TaskType)
                .Include(e => e.ProjectTaskPerformers)
                .ThenInclude(e => e.User)
                .ToListAsync();
        }

        public async Task<List<ProjectTask>> ProjectTasksByUserIdAsync(int userId)
        {
            return await _context.ProjectTasks
                .Where(e => e.ProjectTaskPerformers.Any(ptp => !ptp.Deleted && ptp.UserId.Equals(userId)) 
                    && !e.Deleted && !e.Project.Deleted && !e.Group.Deleted && !e.TaskStatus.Deleted && !e.TaskStatus.Deleted)
                .Include(e => e.Project)
                .Include(e => e.TaskStatus)
                .Include(e => e.Group)
                .Include(e => e.TaskType)
                .Include(e => e.ProjectTaskPerformers)
                .ThenInclude(e => e.User)
                .ToListAsync();
        }
    }
}