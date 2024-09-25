using ActionItems.Intefaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderGo.Models;

namespace OrderGo.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) 
        { 
            _authService = authService;
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // This will return the login view when accessed by a GET request.
        }
        
        [HttpPost]
        public IActionResult Login(LoginRequest loginRequest)
        {
            string token = _authService.Login(loginRequest);
            if (string.IsNullOrEmpty(token))
            {
                // Handle failed login
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }

            // Redirect or handle the successful login scenario
         
            return RedirectToAction("Index", "Home");
        }
        [Route("/")]
        [HttpGet]
        public IActionResult AddUser() {
            return View();
        }
        [HttpPost]
        public IActionResult AddUser(User user)
        {
            if (ModelState.IsValid)
            {
                var addedUser = _authService.AddUser(user);
                return RedirectToAction("UserList");
            }

            // If model validation fails
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddRole(Role role)
        {
            if (ModelState.IsValid)
            {
                var addedRole = _authService.AddRole(role);
                return RedirectToAction("RoleList");
            }

            return View(role);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddUserRole(AssignRole assignRole)
        {
            if (ModelState.IsValid)
            {
                bool isRoleAssigned = _authService.AssignRoleToUser(assignRole);
                if (isRoleAssigned)
                {
                    return RedirectToAction("UserRoleList");
                }
            }

            return View(assignRole);
        }
    }
}

