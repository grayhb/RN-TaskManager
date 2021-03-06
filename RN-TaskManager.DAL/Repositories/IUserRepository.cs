﻿using RN_TaskManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RN_TaskManager.DAL.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<IList<User>> GetUsersAsync();

        Task<IList<User>> GetUsersByGroupIdAsync(int groupId);

    }
}
