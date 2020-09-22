using RN_TaskManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RN_TaskManager.DAL.Repositories
{
    public interface IProjectTaskPerformerRepository : IBaseRepository<ProjectTaskPerformer>
    {
        Task<IList<ProjectTaskPerformer>> GetItemsByGroupIdAsync(int projectTaskId);
    }
}
