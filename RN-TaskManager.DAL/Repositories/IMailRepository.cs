using System.Collections.Generic;
using RN_TaskManager.Models;
using System.Threading.Tasks;

namespace RN_TaskManager.DAL.Repositories
{
    public interface IMailRepository : IBaseRepository<Mail>
    {
        Task CreateMailsAsync();

        Task SendMails();

        /// <summary>
        /// Создание письма о новой задаче для пользователя
        /// </summary>
        Task CreateMailForNewPerformerAsync(User user, ProjectTask projectTask);

        /// <summary>
        /// Создание письма о новой задаче для группы пользователей
        /// </summary>
        /// <param name="users">Список пользователей</param>
        /// <param name="projectTask">Задача</param>
        Task CreateMailsForNewPerformersAsync(List<User> users, ProjectTask projectTask);

    }
}
