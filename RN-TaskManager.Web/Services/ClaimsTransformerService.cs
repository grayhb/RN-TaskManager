using Microsoft.AspNetCore.Authentication;
using RN_TaskManager.DAL.Repositories;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RN_TaskManager.Web.Services
{
    public class ClaimsTransformerService : IClaimsTransformation
    {
        private readonly IUserRepository _userRepository;

        public ClaimsTransformerService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var claims = new List<Claim>();

            ClaimsIdentity identity = (ClaimsIdentity)principal.Identity;

            string userLogin = identity.Name.Split("\\")[1].ToLower();

            var userInfo = await _userRepository.FindAsync(e => e.Login.ToLower().Equals(userLogin));

            if (userInfo != null && userInfo.Count > 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Users"));
            }

            ////Разработчики
            //if (_configuration.GetValue<string>("Developers").Contains(userLogin))
            //    claims.Add(new Claim(ClaimTypes.Role, "Developer"));

            ////Группа САПР
            //if (userInfo.DepartmentName.Contains(_configuration.GetValue<string>("SAPR")))
            //    claims.Add(new Claim(ClaimTypes.Role, "SAPR"));

            if (claims != null && claims.Count > 0)
                identity.AddClaims(claims);

            return principal;
        }
    }
}
