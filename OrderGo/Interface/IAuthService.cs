using OrderGo.Models;

namespace ActionItems.Intefaces
{
    public interface IAuthService
    {
        User AddUser(User user);
        string Login(LoginRequest loginRequest);
        Role AddRole(Role role);
        bool AssignRoleToUser(AssignRole assignRole);
    }
}
