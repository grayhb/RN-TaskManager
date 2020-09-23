using RN_TaskManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RN_TaskManager.DAL.Repositories
{
    public interface IProjectRepository : IBaseRepository<Project>
    {
        Task<List<Project>> GetProjectsAsync();
        Task<Project> GetProjectByIdAsync(int id);
    }
}
