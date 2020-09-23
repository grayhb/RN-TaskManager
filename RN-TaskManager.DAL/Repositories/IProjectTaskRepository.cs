using RN_TaskManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RN_TaskManager.DAL.Repositories
{
    public interface IProjectTaskRepository : IBaseRepository<ProjectTask>
    {
        Task<ProjectTask> ProjectTaskByIdAsync(int id);
        Task<List<ProjectTask>> ProjectTasksAsync();
        Task<List<ProjectTask>> ProjectTasksByUserIdAsync(int userId);
    }
}
