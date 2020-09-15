using RN_TaskManager.DAL.Context;
using RN_TaskManager.Models;

namespace RN_TaskManager.DAL.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly RN_TaskManagerContext _context;

        public UserRepository(RN_TaskManagerContext context) : base(context)
        {
            _context = context;
        }

    }
}
