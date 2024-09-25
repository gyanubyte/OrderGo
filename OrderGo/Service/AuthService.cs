using ActionItems.Intefaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrderGo;
using OrderGo.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderGo.Service
{
    public class AuthService : IAuthService
    {
        public readonly OrderContext _orderContext;
        private readonly IConfiguration _Configuration;

        public AuthService(OrderContext orderContext, IConfiguration configuration)
        {
            _orderContext = orderContext;
            _Configuration = configuration;
        }
        public Role AddRole(Role role)
        {
            var addRole = _orderContext.Roles.Add(role);
             _orderContext.SaveChanges();
            return addRole.Entity;
        }

        public User AddUser(User user)
        {
            var addUser = _orderContext.Users.Add(user);
            _orderContext.SaveChanges();
            return addUser.Entity;
        }

        public bool AssignRoleToUser(AssignRole assignRoles)
        {
            List<UserRole> addRoles = new List<UserRole>();
            var userIdExists = _orderContext.Users.SingleOrDefault(user => user.Id == assignRoles.userId);
            if (userIdExists == null)
            {
                throw new Exception("User is Not Valid");
            }
            var result = assignRoles.roleId;
            foreach (int role in assignRoles.roleId)
            {

                var userRole = new UserRole();
                userRole.roleId = role;
                userRole.userId = userIdExists.Id;
                addRoles.Add(userRole);
            }

            if (addRoles.Count > 0)
            {
                _orderContext.UserRoles.AddRange(addRoles);
               // _orderContext.SaveChanges();
                return true;
            }
            return false;
        }

        public string Login(LoginRequest loginRequest)
        {
            if (loginRequest != null && loginRequest.Password != null)
            {
                var userExit = _orderContext.Users.SingleOrDefault(s => s.userName == loginRequest.UserName && s.password == loginRequest.Password);
                if (userExit != null)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _Configuration["Jwt:Subject"]),
                        new Claim("Id",userExit.Id.ToString()),
                        new Claim("userName",userExit.userName)

                    };
                    var userRoles = _orderContext.UserRoles.Where(u => u.userId == userExit.Id).ToList();
                    var roleIds = userRoles.Select(userRole => userRole.roleId).ToList();
                    var roles = _orderContext.Roles.Where(r => roleIds.Contains(r.Id)).ToList();
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.Name));
                    }
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _Configuration["Jwt:Issuer"],
                        _Configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signIn);

                    var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                    return jwtToken;
                }

            }
            return null;
        }
    }
}
