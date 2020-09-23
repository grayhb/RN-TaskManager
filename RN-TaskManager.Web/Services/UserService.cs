using Microsoft.AspNetCore.Http;
using RN_TaskManager.DAL.Repositories;
using RN_TaskManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RN_TaskManager.Web.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        private User _user;

        public UserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public string userLogin
        {
            get
            {
                string userLogin = _httpContextAccessor.HttpContext.User.Identity.Name;

                if (userLogin == null)
                    userLogin = "anonymous";

                if (userLogin.Contains("\\"))
                    userLogin = userLogin.Split("\\")[1];

                return userLogin;
            }
        }
    }
}
