using Microsoft.EntityFrameworkCore;
using RN_TaskManager.DAL.Context;
using RN_TaskManager.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RN_TaskManager.DAL.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly RN_TaskManagerContext _context;

        public UserRepository(RN_TaskManagerContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IList<User>> GetUsersAsync()
        {
            return await _context.Users
                .Include(e => e.Group)
                .Where(e => !e.Deleted)
                .ToListAsync();
        }
    }
}
