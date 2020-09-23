using Microsoft.EntityFrameworkCore;
using RN_TaskManager.DAL.Context;
using RN_TaskManager.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RN_TaskManager.DAL.Repositories
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        private readonly RN_TaskManagerContext _context;

        public ProjectRepository(RN_TaskManagerContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Project> GetProjectByIdAsync(int id)
        {
            return await _context.Projects
                .Include(e => e.Responsible)
                .SingleOrDefaultAsync(e => !e.Deleted && e.ProjectId.Equals(id));
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            return await _context.Projects.Where(e => !e.Deleted)
                .Include(e => e.Responsible)
                .ToListAsync();
        }
    }
}
