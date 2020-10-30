using RN_TaskManager.Models;
using System.Threading.Tasks;

namespace RN_TaskManager.DAL.Repositories
{
    public interface IMailRepository : IBaseRepository<Mail>
    {
        Task CreateMailsAsync();
        Task SendMails();
    }
}
